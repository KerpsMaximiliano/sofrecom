﻿using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportService
    {
        Response<ManagementReportDetail> GetDetail(string serviceId);
        Response<BillingDetail> GetBilling(string serviceId);
        Response<CostDetailModel> GetCostDetail(string serviceId);
        Response UpdateCostDetail(CostDetailModel CostDetail);
        Response UpdateCostDetailMonth(CostDetailMonthModel CostDetail);
        Response<CostDetailMonthModel> GetCostDetailMonth(string pServiceId, int pMonth, int pYear);
        Response DeleteContracted(int ContractedId);
        Response UpdateDates(int id, ManagementReportUpdateDates model);
    }
}
