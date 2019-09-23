using System;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface ICostDetailRepository : IBaseRepository<CostDetail>
    {
        IList<CostDetail> GetByManagementReport(int managementReportId);
        //List<CostDetailType> GetResourceTypes();
        //List<CostDetailSubtype> GetSubtypes(int idType);

        IList<CostDetail> GetByManagementReportAndDates(int managementReportId, DateTime startDate, DateTime endDate);
        CostDetail GetByManagementReportAndMonthYear(int managementReportId, DateTime monthYear);

        void UpdateTotals(CostDetail costDetailMonth);
        void UpdateHasReal(CostDetail costDetailMonth);
        CostDetail GetWithResourceDetails(int managementReportId, DateTime date);
        void Close(CostDetail detailCost);
        List<CostDetailCategories> GetCategories();
        List<CostDetailSubcategories> GetSubcategories();

        void UpdateTotalProvisioned(CostDetail costDetail);
    }
}
