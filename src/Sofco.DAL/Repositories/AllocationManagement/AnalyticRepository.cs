﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
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

        public IList<Allocation> GetResources(int id)
        {
            return context.Allocations.Where(x => x.AnalyticId == id).Include(x => x.Employee).ToList().AsReadOnly();
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
    }
}
