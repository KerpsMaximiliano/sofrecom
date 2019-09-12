﻿using System;
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
                switch (model.Type)
                {
                    case EvalPropType.Billing:
                        billing.EvalPropBillingValue = model.Value;
                        break;
                    case EvalPropType.Expense:
                        billing.EvalPropExpenseValue = model.Value;
                        break;
                    //case EvalPropType.margin:
                    //    billing.EvalPropMarginValue = model.Value;
                    //    break;
                }

                //if (model.Type == EvalPropType.Billing)
                //    billing.EvalPropBillingValue = model.Value;
                //else
                //    billing.EvalPropExpenseValue = model.Value;

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

        public Response ValidateAddResources(int idBilling, IList<ResourceBillingRequestItem> resources)
        {
            var response = new Response();

            var billing = unitOfWork.ManagementReportBillingRepository.Get(idBilling);

            if (billing == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReportBilling.NotFound);
                return response;
            }

            var index = 1;

            foreach (var item in resources)
            {
                if (item.Deleted) continue;

                if (!unitOfWork.UtilsRepository.ExistProfile(item.ProfileId.GetValueOrDefault()))
                {
                    response.AddErrorAndNoTraslate($"Item {index}: Perfil no existe");
                }

                if (!unitOfWork.UtilsRepository.ExistSeniority(item.SeniorityId.GetValueOrDefault()))
                {
                    response.AddErrorAndNoTraslate($"Item {index}: Seniority no existe");
                }

                if (!unitOfWork.PurchaseOrderRepository.Exist(item.EmployeeId.GetValueOrDefault()))
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

            return response;
        }

        public Response<IList<ResourceBillingRequestItem>> AddResources(int idBilling,
            IList<ResourceBillingRequestItem> resources, string hitoId)
        {
            var response = new Response<IList<ResourceBillingRequestItem>> { Data = new List<ResourceBillingRequestItem>() };

            var billing = unitOfWork.ManagementReportBillingRepository.Get(idBilling);

            try
            {
                foreach (var item in resources)
                {
                    if (item.Id == 0)
                    {
                        var domain = GetResourceBillingDomain(billing.Id, item);
                        domain.HitoCrmId = hitoId;
                        unitOfWork.ManagementReportBillingRepository.AddResource(domain);
                    }
                    else
                    {
                        if (item.Deleted)
                        {
                            unitOfWork.ManagementReportBillingRepository.DeleteResource(new ResourceBilling { Id = item.Id, ManagementReportBillingId = billing.Id });
                        }
                        else
                        {
                            var domain = GetResourceBillingDomain(billing.Id, item);
                            domain.Id = item.Id;

                            unitOfWork.ManagementReportBillingRepository.UpdateResource(domain);
                        }
                    }
                }

                if (resources.Any())
                {
                    if (billing.Id == idBilling)
                    {
                        billing.BilledResources = resources.Count(x => !x.Deleted);
                        billing.BilledResourceTotal = resources.Where(x => !x.Deleted).Sum(x => x.SubTotal);

                        unitOfWork.ManagementReportBillingRepository.Update(billing);
                    }
                }

                response.Data.Add(new ResourceBillingRequestItem
                {
                    Id = billing.Id,
                    Quantity = billing.BilledResources,
                    SubTotal = billing.BilledResourceTotal
                });

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

        public Response<IList<ResourceBillingRequestItem>> GetResources(int idBilling)
        {
            var response = new Response<IList<ResourceBillingRequestItem>> { Data = new List<ResourceBillingRequestItem>() };

            var list = unitOfWork.ManagementReportBillingRepository.GetResources(idBilling);

            if (list.Any())
            {
                response.Data = list.Select(x => new ResourceBillingRequestItem
                {
                    Id = x.Id,
                    EmployeeId = x.EmployeeId,
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

        private ResourceBilling GetResourceBillingDomain(int idBilling, ResourceBillingRequestItem item)
        {
            var domain = new ResourceBilling
            {
                Amount = item.Amount,
                ManagementReportBillingId = idBilling,
                MonthHour = item.MonthHour.GetValueOrDefault(),
                ProfileId = item.ProfileId.GetValueOrDefault(),
                EmployeeId = item.EmployeeId.GetValueOrDefault(),
                Quantity = item.Quantity,
                SeniorityId = item.SeniorityId.GetValueOrDefault(),
                SubTotal = item.SubTotal
            };
            return domain;
        }
    }
}
