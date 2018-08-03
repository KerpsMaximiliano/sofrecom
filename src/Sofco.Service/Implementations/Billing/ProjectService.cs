﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models;
using Sofco.Core.Models.Billing;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class ProjectService : IProjectService
    {
        private readonly ISolfacService solfacService;
        private readonly IProjectData projectData;
        private readonly ILogMailer<ProjectService> logger;
        private readonly IUnitOfWork unitOfWork;

        public ProjectService(ISolfacService solfacService,
            IProjectData projectData, 
            ILogMailer<ProjectService> logger,
            IUnitOfWork unitOfWork)
        {
            this.solfacService = solfacService;
            this.projectData = projectData;
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

        public Response<IList<Project>> GetProjects(string serviceId)
        {
            var response = new Response<IList<Project>>();

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
                    .Select(x => new SelectListModel { Id = x.CrmId, Text = x.Name })
                    .OrderBy(x => x.Text)
                    .ToList()
            };
        }

        public Response<Project> GetProjectById(string projectId)
        {
            var response = new Response<Project>();

            var project = unitOfWork.ProjectRepository.GetByIdCrm(projectId);

            if (project == null)
            {
                response.AddError(Resources.Billing.Project.NotFound);
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
                    .Select(x => new OpportunityOption { Id = x.OpportunityId, Text = $"{x.OpportunityNumber} - {x.OpportunityName}", ProjectId = x.CrmId })
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

        private void SetPurchaseOrderValues(Domain.Models.Billing.Solfac solfac, PurchaseOrderWidgetModel newOc)
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
