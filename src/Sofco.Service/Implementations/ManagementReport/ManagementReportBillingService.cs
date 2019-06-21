using System;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.ManagementReport;
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
    }
}
