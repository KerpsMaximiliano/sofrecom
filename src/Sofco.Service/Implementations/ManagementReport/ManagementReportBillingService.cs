using System;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.ManagementReport;
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

        public Response Update(int id, decimal value)
        {
            var response = new Response();

            var billing = unitOfWork.ManagementReportBillingRepository.Get(id);

            if (billing == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReportBilling.NotFound);
                return response;
            }

            try
            {
                billing.ValueEvalProp = value;
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
