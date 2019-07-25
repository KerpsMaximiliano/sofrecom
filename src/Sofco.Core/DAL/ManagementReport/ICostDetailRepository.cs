using System;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface ICostDetailRepository : IBaseRepository<CostDetail>
    {
        IList<CostDetail> GetByManagementReport(int managementReportId);
        List<CostDetailType> GetResourceTypes();
        IList<CostDetail> GetByManagementReportAndDates(int managementReportId, DateTime startDate, DateTime endDate);
        CostDetail GetByManagementReportAndMonthYear(int managementReportId, DateTime monthYear);

        void UpdateTotals(CostDetail costDetailMonth);
        CostDetail GetWithResourceDetails(int managementReportId, DateTime date);
        void Close(CostDetail detailCost);
        List<CostDetailCategories> GetCategories();
        List<CostDetailSubcategories> GetSubcategories();

    }
}
