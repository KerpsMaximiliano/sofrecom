using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;

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
            return context.Allocations.Where(x => x.AnalyticId == id).Include(x => x.Employee).Select(x => x.Employee).Distinct().ToList().AsReadOnly();
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
                .Include(x => x.Analytic).ThenInclude(x => x.Manager)
                .Select(x => new Analytic
                {
                    Id = x.Id,
                    Manager = new User
                    {
                        Id = x.Analytic.Manager.Id,
                        Email = x.Analytic.Manager.Email
                    }
                })
                .ToList();
        }

        public void Close(Analytic analytic)
        {
            context.Entry(analytic).Property("Status").IsModified = true;
        }

        public ICollection<Analytic> GetAllOpenReadOnly()
        {
            return context.Analytics.Where(x => x.Status == AnalyticStatus.Open).ToList();
        }

        public Analytic GetByService(string serviceId)
        {
            return context.Analytics.SingleOrDefault(x => x.ServiceId.Equals(serviceId));
        }

        public List<Analytic> GetByServiceIds(List<string> serviceIds)
        {
            return context.Analytics.Where(x => x.Status == AnalyticStatus.Open
                && serviceIds.Contains(x.ServiceId)).ToList();
        }

        public ICollection<Analytic> GetByClient(string clientId)
        {
            return context.Analytics.Where(x => x.ClientExternalId.Equals(clientId)).ToList();
        }

        public Analytic GetById(int allocationAnalyticId)
        {
            return context.Analytics.Include(x => x.Manager).SingleOrDefault(x => x.Id == allocationAnalyticId);
        }

        public ICollection<Analytic> GetAnalyticsByManagerId(int managerId)
        {
            return context.Analytics.Where(x => x.ManagerId == managerId && x.Status == AnalyticStatus.Open).ToList();
        }

        public List<Analytic> GetByManagerId(int managerId)
        {
            return context.Analytics.Where(x => x.ManagerId == managerId).ToList();
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

        public IList<Analytic> GetAnalyticsLiteByEmployee(int employeeId)
        {
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId && 
                            x.StartDate.Date == new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1) && 
                            x.Percentage > 0)
                .Include(x => x.Analytic)
                .Select(x => new Analytic
                {
                    Id = x.AnalyticId,
                    Title = x.Analytic.Title,
                    Name = x.Analytic.Name
                })
                .ToList();
        }

        public Analytic GetByTitle(string title)
        {
            return context.Analytics.SingleOrDefault(x => x.Title == title);
        }

        public List<Analytic> GetBySearchCriteria(AnalyticSearchParameters searchCriteria)
        {
            IQueryable<Analytic> query = context.Analytics;

            if (searchCriteria.AnalyticId > 0)
            {
                query = query.Where(x => x.Id == searchCriteria.AnalyticId);
            }

            if (!string.IsNullOrEmpty(searchCriteria.CustomerId))
            {
                query = query.Where(x => x.ClientExternalId == searchCriteria.CustomerId);
            }

            if (!string.IsNullOrEmpty(searchCriteria.ServiceId))
            {
                query = query.Where(x => x.ServiceId == searchCriteria.ServiceId);
            }

            if (searchCriteria.AnalyticStatusId > 0)
            {
                query = query.Where(x => (int)x.Status == searchCriteria.AnalyticStatusId);
            }

            if (searchCriteria.ManagerId > 0)
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
                    .ThenInclude(x => x.Manager)
                .Select(x => x.Analytic)
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
    }
}
