using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.ManagementReport
{
    public class MarginTrackingService : IMarginTrackingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<MarginTrackingService> logger;

        public MarginTrackingService(IUnitOfWork unitOfWork,
            ILogMailer<MarginTrackingService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response<IList<MarginTrackingModel>> Get(int managementReportId)
        {
            var response = new Response<IList<MarginTrackingModel>> { Data = new List<MarginTrackingModel>() };

            var managementReport = unitOfWork.ManagementReportRepository.Get(managementReportId);

            var billings =
                unitOfWork.ManagementReportBillingRepository.GetByManagementReportAndDates(managementReportId,
                    managementReport.StartDate.Date, managementReport.EndDate.Date);

            var costs = unitOfWork.CostDetailRepository.GetByManagementReportAndDates(managementReportId,
                managementReport.StartDate.Date, managementReport.EndDate.Date);

            for (DateTime date = managementReport.StartDate.Date; date.Date <= managementReport.EndDate.Date; date = date.AddMonths(1))
            {
                var itemModel = new MarginTrackingModel();

                itemModel.Date = date;

                var billing = billings.SingleOrDefault(x => x.MonthYear.Date == date);
                var cost = costs.SingleOrDefault(x => x.MonthYear.Date == date);

                CalculatePercentageExpected(billing, itemModel);


          
            }

            return response;
        }

        private void CalculatePercentageExpected(ManagementReportBilling billing, MarginTrackingModel itemModel)
        {
            if (billing != null && billing.EvalPropBillingValue != 0)
            {
                itemModel.PercentageExpected = (billing.EvalPropBillingValue - billing.EvalPropExpenseValue) /
                                               billing.EvalPropBillingValue;
            }
        }
    }
}
