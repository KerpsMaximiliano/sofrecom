using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class AnalyticRepository : BaseRepository<Analytic>, IAnalyticRepository
    {
        public AnalyticRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int id)
        {
            return context.Analytics.Any(x => x.Id == id);
        }

        public IList<Allocation> GetTimelineResources(int id, DateTime startDate, DateTime endDate)
        {
            return context.Allocations.Where(x => x.AnalyticId == id && x.StartDate >= startDate && x.StartDate <= endDate && x.Percentage > 0).Include(x => x.Employee).ToList().AsReadOnly();
        }

        public IList<Employee> GetResources(int id)
        {
            var now = DateTime.UtcNow.AddMonths(-3);

            return context.Allocations.Where(x => x.AnalyticId == id && x.Percentage > 0 && (x.Employee.EndDate == null || x.Employee.EndDate.Value.Date >= now.Date))
                .Include(x => x.Employee)
                .Select(x => x.Employee)
                .Distinct()
                .ToList()
                .AsReadOnly();
        }

        public IList<Employee> GetResources(int id, DateTime startDate, DateTime endDate)
        {
            var query = (from emp in context.Employees
                join allocation in context.Allocations on emp.Id equals allocation.EmployeeId
                where allocation.Percentage > 0 && allocation.AnalyticId == id && !emp.EndDate.HasValue &&
                      (allocation.StartDate.Date == startDate || allocation.StartDate.Date == endDate)
                select emp)
                .Include(x => x.Allocations);
                        
            return query.Distinct().ToList();
        }

        public IList<Employee> GetResources(IList<int> ids, DateTime startDate, DateTime endDate)
        {
            var dateFrom = new DateTime(startDate.Year, startDate.Month, 1);
            var dateTo = new DateTime(endDate.Year, endDate.Month, 1);

            var query = (from emp in context.Employees
                    join allocation in context.Allocations on emp.Id equals allocation.EmployeeId
                    where allocation.Percentage > 0 && ids.Contains(allocation.AnalyticId) && !emp.EndDate.HasValue &&
                          (allocation.StartDate.Date == dateFrom.Date || allocation.StartDate.Date == dateTo.Date)
                    select emp)
                .Include(x => x.Allocations).ThenInclude(x => x.Analytic);

            return query.Distinct().ToList();
        }

        public Analytic GetLastAnalytic(int costCenterId)
        {
            return context.Analytics.Where(x => x.CostCenterId == costCenterId).OrderByDescending(x => x.TitleId).Include(x => x.CostCenter).FirstOrDefault();
        }

        public bool ExistTitle(string analyticTitle)
        {
            return context.Analytics.Any(x => x.Title.Equals(analyticTitle));
        }

        public IList<Analytic> GetAnalyticsByEmployee(int employeeId)
        {
            var today = DateTime.UtcNow;

            return context.Allocations
                .Where(x => x.EmployeeId == employeeId && x.StartDate.Month == today.Month && x.StartDate.Year == today.Year)
                .Include(x => x.Analytic)
                    .ThenInclude(x => x.Manager)
                .Include(x => x.Analytic)
                    .ThenInclude(x => x.Sector)
                .Select(x => new Analytic
                {
                    Id = x.Id,
                    ManagerId = x.Analytic.ManagerId,
                    Manager = new User
                    {
                        Id = x.Analytic.Manager.Id,
                        Email = x.Analytic.Manager.Email
                    },
                    Sector = new Sector
                    {
                        Id = x.Analytic.SectorId,
                        ResponsableUserId = x.Analytic.Sector.ResponsableUserId
                    }
                })
                .ToList();
        }

        public void Close(Analytic analytic)
        {
            context.Entry(analytic).Property("Status").IsModified = true;
            context.Entry(analytic).Property("ClosedBy").IsModified = true;
            context.Entry(analytic).Property("ClosedAt").IsModified = true;
        }

        public ICollection<Analytic> GetAllOpenReadOnly()
        {
            return context.Analytics.Where(x => x.Status == AnalyticStatus.Open).ToList();
        }

        public List<AnalyticLiteModel> GetAllOpenAnalyticLite()
        {
            return context.Analytics
                .Select(s => new AnalyticLiteModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Title = s.Title,
                    AccountId = s.AccountId,
                    AccountName = s.AccountName
                }).ToList();
        }

        public Analytic GetByService(string serviceId)
        {
            return context.Analytics.Include(x => x.Manager).Include(x => x.ManagementReport).SingleOrDefault(x => x.ServiceId.Equals(serviceId));
        }

        public Analytic GetByServiceWithManagementReport(string serviceId)
        {
            return context.Analytics.
                Include(x => x.Manager)
                .Include(x => x.ManagementReport)
                .ThenInclude(x => x.CostDetails)
                    .ThenInclude(x => x.CostDetailOthers)
                        .ThenInclude(y => y.CostDetailSubcategory)
                .Include(x => x.ManagementReport)
                    .ThenInclude(x => x.CostDetails)
                        .ThenInclude(x => x.CostDetailProfiles)
                .Include(x => x.ManagementReport)
                    .ThenInclude(x => x.CostDetails)
                        .ThenInclude(x => x.CostDetailResources)
                .SingleOrDefault(x => x.ServiceId.Equals(serviceId));
        }

        public ICollection<Analytic> GetAllReadOnlyWithManagementReport()
        {
            return context.Analytics.Include(x => x.ManagementReport).ToList().AsReadOnly();
        }

        public bool IsClosed(int analyticId)
        {
            return context.Analytics.Any(x =>
                x.Id == analyticId && (x.Status == AnalyticStatus.Close || x.Status == AnalyticStatus.CloseToExpenses));
        }

        public ICollection<Analytic> GetByManagerIdAndDirectorId(int managerId)
        {
            return context.Analytics.Include(x => x.Sector).Where(x => (x.ManagerId == managerId || x.Sector.ResponsableUserId == managerId) && x.Status == AnalyticStatus.Open).ToList();
        }

        public List<Employee> GetResources(int id, Tuple<DateTime, DateTime> periodExcludeDays, Tuple<DateTime, DateTime> period)
        {
            var startDate = periodExcludeDays.Item1.Date;
            var endDate = periodExcludeDays.Item2.Date;

            var startDateWithDay = period.Item1.Date;
            var endDateWithDay = period.Item2.Date;

            var query = (from emp in context.Employees
                    join allocation in context.Allocations on emp.Id equals allocation.EmployeeId
                    where allocation.Percentage > 0 && allocation.AnalyticId == id &&
                          (!emp.EndDate.HasValue || (emp.EndDate.HasValue && emp.EndDate > startDateWithDay.Date || emp.EndDate < endDateWithDay.Date)) &&
                          (allocation.StartDate.Date == startDate || allocation.StartDate.Date == endDate)
                    select emp)
                .Include(x => x.Allocations);

            return query.Distinct().ToList();
        }

        public IList<Analytic> GetActiveByIds(IList<int> ids)
        {
            return context.Analytics.Where(x => x.Status == AnalyticStatus.Open && ids.Contains(x.Id)).ToList();
        }

        public Analytic GetByServiceForManagementReport(string serviceId)
        {
            return context.Analytics
                .Include(x => x.ServiceType)
                .Include(x => x.Technology)
                .Include(x => x.Solution)
                .Include(x => x.Sector)
                .Include(x => x.Manager)
                .Include(x => x.ManagementReport)
                .SingleOrDefault(x => x.ServiceId.Equals(serviceId));
        }

        public User GetDirector(int analyticId)
        {
            return context.Analytics
                .Include(x => x.Sector)
                    .ThenInclude(x => x.ResponsableUser)
                .SingleOrDefault(x => x.Id == analyticId)?.Sector?.ResponsableUser;
        }

        public IList<AnalyticLiteModel> GetByDirectorId(int currentUserId)
        {
            return context.Analytics
                .Include(x => x.Sector)
                .Where(x => x.Sector.ResponsableUserId == currentUserId)
                .Select(s => new AnalyticLiteModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Title = s.Title,
                    AccountName = s.AccountName,
                    AccountId = s.AccountId
                }).ToList();
        }

        public User GetManager(int analyticId)
        {
            return context.Analytics
                .Include(x => x.Manager)
                .SingleOrDefault(x => x.Id == analyticId)?.Manager;
        }

        public Analytic GetByProyectId(string projectId)
        {
            var project = context.Projects.SingleOrDefault(x => x.CrmId.Equals(projectId));

            if (project != null)
            {
                var analytic = context.Analytics.SingleOrDefault(x => x.ServiceId.Equals(project.ServiceId));

                if (analytic != null)
                {
                    return analytic;
                }
            }

            return null;
        }

        public List<Analytic> GetByServiceIds(List<string> serviceIds)
        {
            return context.Analytics.Where(x => x.Status == AnalyticStatus.Open
                && serviceIds.Contains(x.ServiceId)).ToList();
        }

        public ICollection<Analytic> GetByClient(string clientId, bool onlyActives)
        {
            if (onlyActives)
            {
                return context.Analytics.Where(x => x.AccountId.Equals(clientId) && x.Status == AnalyticStatus.Open).ToList();
            }
            else
            {
                return context.Analytics.Where(x => x.AccountId.Equals(clientId)).ToList();
            }
        }

        public Analytic GetById(int allocationAnalyticId)
        {
            return context.Analytics.Include(x => x.Manager).Include(x=>x.Refunds).SingleOrDefault(x => x.Id == allocationAnalyticId);
        }

        public ICollection<Analytic> GetAnalyticsByManagerId(int managerId)
        {
            return context.Analytics.Where(x => x.ManagerId == managerId && x.Status == AnalyticStatus.Open).ToList();
        }

        public List<AnalyticLiteModel> GetAnalyticLiteByManagerId(int managerId)
        {
            return context.Analytics
                .Where(x => x.ManagerId == managerId
                && x.Status == AnalyticStatus.Open)
                .Select(s => new AnalyticLiteModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Title = s.Title
                }).ToList();
        }

        public AnalyticLiteModel GetAnalyticLiteById(int id)
        {
            return context.Analytics.Where(s => s.Id == id).Select(s => new AnalyticLiteModel
            {
                Id = s.Id,
                Name = s.Name,
                Title = s.Title
            }).FirstOrDefault();

        }

        public List<AnalyticLiteModel> GetAnalyticLiteByIds(List<int> ids)
        {
            return context.Analytics
                .Where(s => ids.Contains(s.Id))
                .Select(s => new AnalyticLiteModel { Id = s.Id, Name = s.Name, Title = s.Title })
                .ToList();
        }

        public IList<Analytic> GetAnalyticsRequestNote(int userId)
        {
            var analyticsByManagers = context.Analytics
                .Where(x => x.ManagerId.GetValueOrDefault() == userId && x.Status == AnalyticStatus.Open)
                .Select(x => new Analytic
                {
                    Id = x.Id,
                    Title = x.Title,
                    Name = x.Name
                })
                .ToList();

            var sectors = context.Sectors.Where(x => x.ResponsableUserId == userId).Select(x => new Sector
            {
                Analytics = x.Analytics,
                ResponsableUser = x.ResponsableUser,
                ResponsableUserId = x.ResponsableUserId,

            }).ToList();

            var analyticsByDirector = new List<Analytic>();

            foreach (Sector sector in sectors)
            {
                foreach(Analytic analytic in sector.Analytics)
                {
                    analyticsByDirector.Add(analytic);
                }
            }

            return analyticsByDirector.Union(analyticsByManagers).ToList();
        }

        public IList<Analytic> GetAnalyticsLiteByEmployee(int employeeId, int userId, DateTime dateFrom, DateTime dateTo)
        {
            var analyticsByAllocations = context.Allocations
                .Where(x =>  x.EmployeeId == employeeId 
                             && x.Percentage > 0 
                             && x.Analytic.Status == AnalyticStatus.Open 
                             && (x.StartDate.Date == dateFrom || x.StartDate.Date == dateTo))
                .Include(x => x.Analytic)
                .Select(x => new Analytic
                {
                    Id = x.AnalyticId,
                    Title = x.Analytic.Title,
                    Name = x.Analytic.Name
                })
                .ToList();

            var analyticsByManagers = context.Analytics
                .Where(x => x.ManagerId.GetValueOrDefault() == userId && x.Status == AnalyticStatus.Open)
                .Select(x => new Analytic
                {
                    Id = x.Id,
                    Title = x.Title,
                    Name = x.Name
                })
                .ToList();

            return analyticsByAllocations.Union(analyticsByManagers).ToList();
        }

        public Analytic GetByTitle(string title)
        {
            return context.Analytics.SingleOrDefault(x => x.Title == title);
        }

        public List<Analytic> GetBySearchCriteria(AnalyticSearchParameters searchCriteria)
        {
            IQueryable<Analytic> query = context.Analytics.Include(x => x.ManagementReport).Include(x => x.Activity).Include(x => x.Sector);

            if (searchCriteria.AnalyticId.HasValue && searchCriteria.AnalyticId.Value > 0)
            {
                query = query.Where(x => x.Id == searchCriteria.AnalyticId);
            }

            if (!string.IsNullOrEmpty(searchCriteria.CustomerId))
            {
                query = query.Where(x => x.AccountId == searchCriteria.CustomerId);
            }

            if (!string.IsNullOrEmpty(searchCriteria.ServiceId))
            {
                query = query.Where(x => x.ServiceId == searchCriteria.ServiceId);
            }

            if (searchCriteria.AnalyticStatusId.HasValue && searchCriteria.AnalyticStatusId.Value > 0)
            {
                query = query.Where(x => (int)x.Status == searchCriteria.AnalyticStatusId);
            }

            if (searchCriteria.ManagerId.HasValue && searchCriteria.ManagerId.Value > 0)
            {
                query = query.Where(x => x.ManagerId == searchCriteria.ManagerId);
            }

            return query.ToList();
        }

        public List<Analytic> GetForReport(List<int> analytics)
        {
            return context.Analytics
                .Include(x => x.Activity)
                .Include(x => x.CostCenter)
                .Include(x => x.ClientGroup)
                .Include(x => x.Manager)
                .Include(x => x.CommercialManager)
                .Include(x => x.Sector)
                .Include(x => x.ServiceType)
                .Include(x => x.SoftwareLaw)
                .Include(x => x.Solution)
                .Include(x => x.Technology)
                .Where(x => analytics.Contains(x.Id))
                .ToList();
        }

        public IList<Analytic> GetByPurchaseOrder(int purchaseOrderId)
        {
            return context.PurchaseOrderAnalytics
                .Where(x => x.PurchaseOrderId == purchaseOrderId)
                .Include(x => x.Analytic)
                .Select(x => x.Analytic)
                .Include(x => x.Manager)
                .ToList();

        }

        public bool ExistManagerId(int managerId)
        {
            return context.Analytics.Any(x => x.ManagerId == managerId && x.Status == AnalyticStatus.Open);
        }

        public void UpdateDaf(Analytic analytic)
        {
            context.Entry(analytic).Property("SoftwareLawId").IsModified = true;
            context.Entry(analytic).Property("ActivityId").IsModified = true;
        }

        public List<Analytic> GetByAllocations(int employeeId, DateTime dateFrom, DateTime dateTo)
        {
            var analyticIds = context.Allocations
                .Where(x => x.EmployeeId == employeeId && (x.StartDate.Date == dateFrom.Date || x.StartDate.Date == dateTo.Date))
                .Select(x => x.AnalyticId)
                .Distinct()
                .ToList();

            return context.Analytics
                .Include(x => x.Manager)
                .Where(x => analyticIds.Contains(x.Id))
                .Distinct()
                .ToList();
        }

        
    }
}
