using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Implementations.Billing
{
    public class ProjectService : IProjectService
    {
        private readonly ISolfacService solfacService;
        private readonly CrmConfig crmConfig;
        private readonly ICrmHttpClient client;
        private readonly IProjectData projectData;
        private readonly ILogMailer<ProjectService> logger;
        private readonly IUnitOfWork unitOfWork;

        public ProjectService(ISolfacService solfacService, IOptions<CrmConfig> crmOptions,
            IProjectData projectData, ICrmHttpClient client, ILogMailer<ProjectService> logger, IUnitOfWork unitOfWork)
        {
            this.solfacService = solfacService;
            crmConfig = crmOptions.Value;
            this.projectData = projectData;
            this.client = client;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public IList<CrmProjectHito> GetHitosByProject(string projectId)
        {
            var hitos = solfacService.GetHitosByProject(projectId);

            var crmProjectHitos = projectData.GetHitos(projectId);

            foreach (var hitoCrm in crmProjectHitos)
            {
                var existHito = hitos.SingleOrDefault(x => x.ExternalHitoId == hitoCrm.Id);

                if (existHito != null)
                {
                    hitoCrm.SolfacId = existHito.SolfacId;
                }

                if (!hitoCrm.Status.Equals("Pendiente")
                    && !hitoCrm.Status.Equals("Proyectado") || existHito != null)
                {
                    hitoCrm.Billed = true;
                }

                if (hitoCrm.Status.Equals("A ser facturado"))
                {
                    hitoCrm.Status = HitoStatus.ToBeBilled.ToString();
                }
            }

            return crmProjectHitos;
        }

        public Response<IList<CrmProject>> GetProjects(string serviceId)
        {
            var response = new Response<IList<CrmProject>>();

            if (string.IsNullOrWhiteSpace(serviceId)) return response;

            try
            {
                response.Data = projectData.GetProjects(serviceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response<IList<SelectListModel>> GetProjectsOptions(string serviceId)
        {
            var result = GetProjects(serviceId).Data;

            return new Response<IList<SelectListModel>>
            {
                Data = result
                    .Select(x => new SelectListModel { Id = x.Id, Text = x.Nombre })
                    .OrderBy(x => x.Text)
                    .ToList()
            };
        }

        public Response<CrmProject> GetProjectById(string projectId)
        {
            var url = $"{crmConfig.Url}/api/project/{projectId}";

            var project = client.Get<CrmProject>(url).Data;

            var response = new Response<CrmProject>();

            if (project.Id.Equals("00000000-0000-0000-0000-000000000000"))
            {
                response.Messages.Add(new Message(Resources.Billing.Project.NotFound, MessageType.Error));
                return response;
            }

            response.Data = project;
            return response;
        }

        public Response<IList<OpportunityOption>> GetOpportunities(string serviceId)
        {
            var result = GetProjects(serviceId).Data;

            return new Response<IList<OpportunityOption>>
            {
                Data = result
                    .Select(x => new OpportunityOption { Id = x.OpportunityId, Text = $"{x.OpportunityNumber} - {x.OpportunityName}", ProjectId = x.Id })
                    .OrderBy(x => x.Text)
                    .ToList()
            };
        }

        public IList<PurchaseOrderWidgetModel> GetPurchaseOrders(string projectId)
        {
            var list = new List<PurchaseOrderWidgetModel>();

            var solfacs = unitOfWork.SolfacRepository.GetByProjectWithPurchaseOrder(projectId);

            foreach (var solfac in solfacs)
            {
                if(solfac.PurchaseOrder == null) continue;

                var ocs = list.Where(x => x.PurchaseOrder.Equals(solfac.PurchaseOrder.Number));

                if (!ocs.Any())
                {
                    foreach (var detail in solfac.PurchaseOrder.AmmountDetails)
                    {
                        var newOc = new PurchaseOrderWidgetModel { PurchaseOrder = solfac.PurchaseOrder.Number, Balance = detail.Balance, Currency = detail.Currency.Text };

                        if (detail.CurrencyId == solfac.CurrencyId)
                            SetPurchaseOrderValues(solfac, newOc);

                        list.Add(newOc);
                    }
                }
                else
                {
                    foreach (var detail in solfac.PurchaseOrder.AmmountDetails)
                    {
                        if (detail.CurrencyId == solfac.CurrencyId)
                        {
                            var oc = list.SingleOrDefault(x => x.PurchaseOrder.Equals(solfac.PurchaseOrder.Number) && x.Currency.Equals(detail.Currency.Text));
                            SetPurchaseOrderValues(solfac, oc);
                        }
                    }
                }
            }

            return list;
        }

        private void SetPurchaseOrderValues(Model.Models.Billing.Solfac solfac, PurchaseOrderWidgetModel newOc)
        {
            switch (solfac.Status)
            {
                case SolfacStatus.InvoicePending:
                case SolfacStatus.ManagementControlRejected:
                case SolfacStatus.RejectedByDaf:
                case SolfacStatus.PendingByManagementControl: newOc.BillingPendingAmmount += solfac.TotalAmount; break;

                case SolfacStatus.Invoiced: newOc.CashPendingAmmount += solfac.TotalAmount; break;

                case SolfacStatus.AmountCashed: newOc.AmmountCashed += solfac.TotalAmount; break;
            }
        }
    }
}
