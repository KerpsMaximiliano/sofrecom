﻿using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportStaffService
    {
        Response<ManagementReportStaffDetail> GetDetail(int id);
        Response<BudgetItem> AddBudget(int id, BudgetItem model);
        Response<BudgetItem> UpdateBudget(int id, BudgetItem model);
        Response DeleteBudget(int id, int budgetId);
    }
}
