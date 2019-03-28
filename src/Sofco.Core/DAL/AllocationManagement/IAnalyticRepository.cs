using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IAnalyticRepository : IBaseRepository<Analytic>
    {
        bool Exist(int id);

        IList<Allocation> GetTimelineResources(int id, DateTime startDate, DateTime endDate);

        IList<Employee> GetResources(int id);

        IList<Employee> GetResources(int id, DateTime startDate, DateTime endDate);

        Analytic GetLastAnalytic(int costCenterId);

        bool ExistTitle(string analyticTitle);

        IList<Analytic> GetAnalyticsByEmployee(int employeeId);

        void Close(Analytic analytic);

        ICollection<Analytic> GetAllOpenReadOnly();

        List<AnalyticLiteModel> GetAllOpenAnalyticLite();

        Analytic GetByService(string serviceId);

        List<Analytic> GetByServiceIds(List<string> serviceIds);

        ICollection<Analytic> GetByClient(string clientId, bool onlyActives);

        Analytic GetById(int allocationAnalyticId);

        ICollection<Analytic> GetAnalyticsByManagerId(int managerId);

        List<AnalyticLiteModel> GetAnalyticLiteByManagerId(int managerId);

        AnalyticLiteModel GetAnalyticLiteById(int id);

        List<AnalyticLiteModel> GetAnalyticLiteByIds(List<int> ids);

        IList<Analytic> GetAnalyticsLiteByEmployee(int employeeId, int userId, DateTime dateFrom, DateTime dateTo);

        Analytic GetByTitle(string title);

        List<Analytic> GetBySearchCriteria(AnalyticSearchParameters searchCriteria);

        List<Analytic> GetForReport(List<int> analytics);

        IList<Analytic> GetByPurchaseOrder(int purchaseOrderId);

        bool ExistManagerId(int managerId);

        void UpdateDaf(Analytic analytic);

        List<Analytic> GetByAllocations(int employeeId, DateTime dateFrom, DateTime dateTo);
        Analytic GetByServiceForManagementReport(string serviceId);
        User GetDirector(int analyticId);
    }
}
