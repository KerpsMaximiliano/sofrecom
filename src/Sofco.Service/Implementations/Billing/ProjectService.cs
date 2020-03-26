using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models;
using Sofco.Core.Models.Billing;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Jobs;
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
        private readonly IProjectUpdateJobService projectUpdateJobService;

        public ProjectService(ISolfacService solfacService,
            IProjectData projectData, 
            ILogMailer<ProjectService> logger,
            IProjectUpdateJobService projectUpdateJobService,
            IUnitOfWork unitOfWork)
        {
            this.solfacService = solfacService;
            this.projectData = projectData;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.projectUpdateJobService = projectUpdateJobService;
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

        public Response<IList<ProjectModel>> GetProjects(string serviceId)
        {
            var response = new Response<IList<ProjectModel>>();

            if (string.IsNullOrWhiteSpace(serviceId)) return response;

            try
            {
                //response.Data = projectData.GetProjects(serviceId);

                IList<Project> proyects = projectData.GetProjects(serviceId);

                List<ProjectModel> projectsModel = proyects.Select(x => new ProjectModel(x)).ToList();

                foreach (var project in projectsModel)
                {
                    var ocInfo = GetPurchaseOrders(project.CrmId);

                    project.Billings = new List<ProjectBillingItem>();

                    foreach (var purchaseOrderWidgetModel in ocInfo)
                    {
                        var item = project.Billings.SingleOrDefault(x => x.Currency.Equals(purchaseOrderWidgetModel.Currency));

                        if (item != null)
                        {
                            item.Billed += purchaseOrderWidgetModel.AmmountCashed + purchaseOrderWidgetModel.CashPendingAmmount;
                            item.BillingPending += purchaseOrderWidgetModel.BillingPendingAmmount;
                        }
                        else
                        {
                            var newItem = new ProjectBillingItem
                            {
                                Currency = purchaseOrderWidgetModel.Currency,
                                Billed = purchaseOrderWidgetModel.AmmountCashed + purchaseOrderWidgetModel.CashPendingAmmount,
                                BillingPending = purchaseOrderWidgetModel.BillingPendingAmmount
                            };

                            project.Billings.Add(newItem);
                        }
                    }

                    //var crmProjectHitos = projectData.GetHitos(project.CrmId);

                    //foreach (var hitoCrm in crmProjectHitos)
                    //{
                    //    if (hitoCrm.Status.ToLower().Equals("a ser facturado") || hitoCrm.Status.ToLower().Equals("facturado") 
                    //        || hitoCrm.Status.ToLower().Equals("pagado"))
                    //    {
                    //        project.HitosBilled += hitoCrm.Ammount;
                    //    }

                    //    if (hitoCrm.Status.ToLower().Equals("pendiente"))
                    //    {
                    //        project.HitosPending += hitoCrm.Ammount;
                    //    }
                    //}
                }

                response.Data = projectsModel;
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

            var opp = unitOfWork.OpportunityRepository.GetByCrmId(project.OpportunityId);

            if (opp != null)
            {
                var contact = unitOfWork.ContactRepository.GetByCrmId(opp.ContactId);

                if (contact != null)
                {
                    project.PrincipalContactName = contact.Name;
                    project.PrincipalContactEmail = contact.Email;
                }
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
                        var newOc = new PurchaseOrderWidgetModel {
                            PurchaseOrder = solfac.PurchaseOrder.Number,
                            AdjustmentBalance = detail.AdjustmentBalance,
                            TotalAmmount = detail.Ammount,
                            Currency = detail.Currency.Text
                        };

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

        public Response Update()
        {
            var response = new Response();

            try
            {
                projectUpdateJobService.Execute();
                response.AddSuccess(Resources.Common.UpdateSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var project = unitOfWork.ProjectRepository.Get(id);

            if (project == null)
            {
                response.AddError(Resources.Billing.Project.NotFound);
                return response;
            }

            try
            {
                project.Active = false;
                unitOfWork.ProjectRepository.Update(project);
                unitOfWork.Save();

                response.AddSuccess(Resources.Common.DeleteSuccess);

                projectData.ClearKeys();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void SetPurchaseOrderValues(Solfac solfac, PurchaseOrderWidgetModel newOc)
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
