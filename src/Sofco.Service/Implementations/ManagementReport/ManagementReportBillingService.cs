using System;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Enums;
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

        public Response Update(UpdateValueModel model)
        {
            var response = new Response();

            var billing = unitOfWork.ManagementReportBillingRepository.Get(model.Id);

            if (billing == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReportBilling.NotFound);
                return response;
            }

            try
            {
                if (model.Type == EvalPropType.Billing)
                    billing.EvalPropBillingValue = model.Value;
                else
                    billing.EvalPropExpenseValue = model.Value;
               
                unitOfWork.ManagementReportBillingRepository.Update(billing);
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

        public Response UpdateData(UpdateBillingDataModel model)
        {
            var response = new Response();

            var billing = unitOfWork.ManagementReportBillingRepository.Get(model.Id);

            if (billing == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReportBilling.NotFound);
                return response;
            }

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
