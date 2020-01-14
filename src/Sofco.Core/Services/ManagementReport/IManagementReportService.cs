using Sofco.Core.Models.ManagementReport;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;

namespace Sofco.Core.Services.ManagementReport
{
    public interface IManagementReportService
    {
        Response<ManagementReportDetail> GetDetail(string serviceId);
        Response<BillingDetail> GetBilling(string serviceId);
        Response<CostDetailModel> GetCostDetail(string serviceId);
        //Response<List<CostDetailTypeModel>> GetOtherResources();
        Response<MonthOther> GetOtherTypeAndCostDetail(int idCategory, int idCostDetail);
        Response UpdateCostDetail(CostDetailModel CostDetail);
        Response UpdateCostDetailMonth(CostDetailMonthModel CostDetail);
        Response<CostDetailMonthModel> GetCostDetailMonth(string pServiceId, int pMonth, int pYear);
        Response DeleteContracted(int ContractedId);
        Response DeleteOtherResource(int id);
        Response<CostResourceEmployee> GetCostDetailByEmployee(string serviceId, int employeeId);
        Response UpdateDates(int id, ManagementReportUpdateDates model);
        Response<byte[]> CreateTracingReport(TracingModel tracing);
        Response Send(ManagementReportSendModel model);
        Response Close(ManagementReportCloseModel model);     
        Response<ManagementReportCommentModel> AddComment(ManagementReportAddCommentModel model);
        Response<IList<ManagementReportCommentModel>> GetComments(int id);
        Response DeleteProfile(string guid);
        void InsertUpdateCostDetailResources(IList<CostResourceEmployee> pCostEmployees, IList<CostDetail> costDetails, int managementReportId, bool isReal = false);
        List<CostResourceEmployee> FillCostEmployeesByMonth(int IdAnalytic, IList<MonthHeaderCost> Months, ICollection<CostDetail> costDetails, DateTime startDate, DateTime endDate);
        IList<CostResourceEmployee> AddAnalyticMonthsToEmployees(IList<CostResourceEmployee> pCostEmployees, int managementReportId, DateTime startDateAnalytic, DateTime endDateAnalytic);
        Response UpdateAcumulated(int id, AcumulatedRequest request);
    }
}
