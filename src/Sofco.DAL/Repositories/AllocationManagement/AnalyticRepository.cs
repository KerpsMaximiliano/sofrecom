using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Enums.TimeManagement;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.AllocationManagement;

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
            return context.Allocations.Where(x => x.AnalyticId == id && x.StartDate >= startDate && x.StartDate <= endDate).Include(x => x.Employee).ToList().AsReadOnly();
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
            return context.Allocations
                .Where(x => x.EmployeeId == employeeId)
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

        public ICollection<Analytic> GetByClient(string clientId)
        {
            return context.Analytics.Where(x => x.ClientExternalId.Equals(clientId)).ToList();
        }

        public Analytic GetById(int allocationAnalyticId)
        {
            return context.Analytics.Include(x => x.Manager).SingleOrDefault(x => x.Id == allocationAnalyticId);
        }

        public ICollection<Analytic> GetAnalyticsByManagers(int id)
        {
            return context.Analytics.Where(x => x.ManagerId == id || x.DirectorId == id).ToList();
        }

        public List<Analytic> GetByManagerId(int managerId)
        {
            return context.Analytics.Where(x => x.ManagerId == managerId).ToList();
        }

        public List<AnalyticLiteModel> GetAnalyticLiteByManagerId(int managerId)
        {
            return context.Analytics
                .Where(x => x.ManagerId == managerId || x.DirectorId == managerId
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
    }
}
