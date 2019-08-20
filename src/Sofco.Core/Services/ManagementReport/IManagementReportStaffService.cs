using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportStaffService
    {
        Response<ManagementReportStaffDetail> GetDetail(int id);
        Response<CostDetailStaffMonthModel> GetCostDetailMonth(int id, int month, int year);
        Response<CostDetailStaffModel> GetCostDetailStaff(int managementReportId);
        Response UpdateCostDetailStaff(CostDetailStaffModel pDetailCost);
        Response GetCategories();
        Response Close(ManagementReportCloseModel model);
    }
}
