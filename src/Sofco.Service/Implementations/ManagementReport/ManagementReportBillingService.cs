using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.ManagementReport
{
    public class ManagementReportBillingService : IManagementReportBillingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ManagementReportBillingService> logger;

        public ManagementReportBillingService(IUnitOfWork unitOfWork,
            ILogMailer<ManagementReportBillingService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response<int> Update(UpdateValueModel model)
        {
            var response = new Response<int>();

            var billing = GetBilling(model.ManagementReportId, model.MonthYear, model.Id);

            try
            {
                if (model.Type == EvalPropType.Billing)
                    billing.EvalPropBillingValue = model.Value;
                else
                    billing.EvalPropExpenseValue = model.Value;

                if (billing.Id > 0)
                {
                    unitOfWork.ManagementReportBillingRepository.Update(billing);
                }
                else
                {
                    unitOfWork.ManagementReportBillingRepository.Insert(billing);
                }

                unitOfWork.Save();

                response.Data = billing.Id;

                response.AddSuccess(Resources.ManagementReport.ManagementReportBilling.ValueUpdated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private ManagementReportBilling GetBilling(int managementReportId, DateTime monthYear, int id)
        {
            ManagementReportBilling billing;

            if (id == 0)
            {
                billing = new ManagementReportBilling
                {
                    ManagementReportId = managementReportId,
                    MonthYear = monthYear.Date
                };
            }
            else
            {
                billing = unitOfWork.ManagementReportBillingRepository.Get(id);

                if (billing == null)
                {
                    billing = unitOfWork.ManagementReportBillingRepository.GetByManagementReportIdAndDate(
                        managementReportId, monthYear);

                    if (billing == null)
                    {
                        billing = new ManagementReportBilling
                        {
                            ManagementReportId = managementReportId,
                            MonthYear = monthYear.Date
                        };
                    }
                }
            }

            return billing;
        }

        public Response<int> UpdateData(UpdateBillingDataModel model)
        {
            var response = new Response<int>();

            var billing = GetBilling(model.ManagementReportId, model.MonthYear, model.Id);

            if (model.Type == ReportBillingUpdateDataType.BilledResources && !model.Resources.HasValue)
                response.AddError(Resources.ManagementReport.ManagementReportBilling.ResourcesRequired);

            if (model.Type == ReportBillingUpdateDataType.EvalPropDifference && !model.EvalPropDifference.HasValue)
                response.AddError(Resources.ManagementReport.ManagementReportBilling.EvalPropDifferenceRequired);

            if (response.HasErrors()) return response;

            try
            {
                if (model.Type == ReportBillingUpdateDataType.Comments)
                    billing.Comments = model.Comments;

                if (model.Type == ReportBillingUpdateDataType.BilledResources)
                    billing.BilledResources = model.Resources.GetValueOrDefault();

                if (model.Type == ReportBillingUpdateDataType.EvalPropDifference)
                    billing.EvalPropDifference = model.EvalPropDifference.GetValueOrDefault();

                unitOfWork.ManagementReportBillingRepository.Update(billing);
                unitOfWork.Save();

                response.Data = billing.Id;

                response.AddSuccess(Resources.ManagementReport.ManagementReportBilling.ValueUpdated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response UpdateQuantityResources(int idBilling, int quantityResources)
        {
            var response = new Response();                
            try
            {
                ManagementReportBilling entity = unitOfWork.ManagementReportBillingRepository.Get(idBilling);

                entity.BilledResources = quantityResources;

                unitOfWork.ManagementReportBillingRepository.Update(entity);
                unitOfWork.Save();

                response.AddSuccess(Resources.ManagementReport.ManagementReportBilling.ValueUpdated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<ResourceBillingRequest>> AddResources(int idBilling, IList<ResourceBillingRequest> resources)
        {
            var response = new Response<IList<ResourceBillingRequest>> { Data = new List<ResourceBillingRequest>() };

            var billing = unitOfWork.ManagementReportBillingRepository.Get(idBilling);

            if (billing == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReportBilling.NotFound);
                return response;
            }

            var index = 1;

            foreach (var item in resources)
            {
                if(item.Deleted) continue;

                if (!unitOfWork.UtilsRepository.ExistProfile(item.ProfileId.GetValueOrDefault()))
                {
                    response.AddErrorAndNoTraslate($"Item {index}: Perfil no existe");
                }

                if (!unitOfWork.UtilsRepository.ExistSeniority(item.SeniorityId.GetValueOrDefault()))
                {
                    response.AddErrorAndNoTraslate($"Item {index}: Seniority no existe");
                }

                if (!unitOfWork.PurchaseOrderRepository.Exist(item.PurchaseOrderId.GetValueOrDefault()))
                {
                    response.AddErrorAndNoTraslate($"Item {index}: Orden de compra no existe");
                }

                if (!item.MonthHour.HasValue || item.MonthHour == 0)
                {
                    response.AddErrorAndNoTraslate($"Item {index}: Mes/Hora es requerido");
                }

                if (item.Quantity <= 0)
                {
                    response.AddErrorAndNoTraslate($"Item {index}: Cantidad es requerida");
                }

                if (item.Amount <= 0)
                {
                    response.AddErrorAndNoTraslate($"Item {index}: Monto es requerido");
                }

                index++;
            }

            if (response.HasErrors()) return response;

            try
            {
                var billings = unitOfWork.ManagementReportRepository.GetBillingsByMonthYear(billing.MonthYear, billing.ManagementReportId);

                foreach (var reportBilling in billings)
                {
                    var resourceCount = unitOfWork.ManagementReportBillingRepository.GetResourcesCount(reportBilling.Id);
                    var billingCount = resourceCount;
                    decimal billingSum = 0;

                    foreach (var item in resources)
                    {
                        if (reportBilling.Id != idBilling)
                        {
                            if (!item.Deleted && resourceCount == 0)
                            {
                                var domain = GetResourceBillingDomain(reportBilling.Id, item);

                                unitOfWork.ManagementReportBillingRepository.AddResource(domain);

                                billingCount++;
                                billingSum += domain.SubTotal;
                            }
                        }
                        else
                        {
                            if (item.Id == 0)
                            {
                                var domain = GetResourceBillingDomain(reportBilling.Id, item);

                                unitOfWork.ManagementReportBillingRepository.AddResource(domain);
                            }
                            else
                            {
                                if (item.Deleted)
                                {
                                    unitOfWork.ManagementReportBillingRepository.DeleteResource(new ResourceBilling { Id = item.Id, ManagementReportBillingId = reportBilling.Id });
                                }
                                else
                                {
                                    var domain = GetResourceBillingDomain(reportBilling.Id, item);
                                    domain.Id = item.Id;

                                    unitOfWork.ManagementReportBillingRepository.UpdateResource(domain);
                                }
                            }
                        }
                    }

                    if (resources.Any())
                    {
                        if (reportBilling.Id == idBilling)
                        {
                            reportBilling.BilledResources = resources.Count(x => !x.Deleted);
                            reportBilling.BilledResourceTotal = resources.Where(x => !x.Deleted).Sum(x => x.SubTotal);

                            unitOfWork.ManagementReportBillingRepository.Update(reportBilling);
                        }
                        else
                        {
                            if (resourceCount == 0 && billingCount > 0)
                            {
                                reportBilling.BilledResources = billingCount;
                                reportBilling.BilledResourceTotal = billingSum;

                                unitOfWork.ManagementReportBillingRepository.Update(reportBilling);
                            }
                        }
                    }

                    response.Data.Add(new ResourceBillingRequest
                    {
                        Id = reportBilling.Id,
                        Quantity = reportBilling.BilledResources,
                        SubTotal = reportBilling.BilledResourceTotal
                    });
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.ManagementReport.ManagementReportBilling.ResourcesUpdated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<ResourceBillingRequest>> GetResources(int idBilling)
        {
            var response = new Response<IList<ResourceBillingRequest>> { Data = new List<ResourceBillingRequest>() };

            var list = unitOfWork.ManagementReportBillingRepository.GetResources(idBilling);

            if (list.Any())
            {
                response.Data = list.Select(x => new ResourceBillingRequest
                {
                    Id = x.Id,
                    PurchaseOrderId = x.PurchaseOrderId,
                    Amount = x.Amount,
                    SubTotal = x.SubTotal,
                    SeniorityId = x.SeniorityId,
                    Quantity = x.Quantity,
                    MonthHour = x.MonthHour,
                    ProfileId = x.ProfileId
                })
                .ToList();
            }

            return response;
        }

        private ResourceBilling GetResourceBillingDomain(int idBilling, ResourceBillingRequest item)
        {
            var domain = new ResourceBilling
            {
                Amount = item.Amount,
                ManagementReportBillingId = idBilling,
                MonthHour = item.MonthHour.GetValueOrDefault(),
                ProfileId = item.ProfileId.GetValueOrDefault(),
                PurchaseOrderId = item.PurchaseOrderId.GetValueOrDefault(),
                Quantity = item.Quantity,
                SeniorityId = item.SeniorityId.GetValueOrDefault(),
                SubTotal = item.SubTotal
            };
            return domain;
        }
    }
}
