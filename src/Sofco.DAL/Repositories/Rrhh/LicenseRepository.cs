using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Rrhh;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories.Rrhh
{
    public class LicenseRepository : BaseRepository<License>, ILicenseRepository
    {
        public LicenseRepository(SofcoContext context) : base(context)
        {
        }

        public void AddFile(LicenseFile licenseFile)
        {
            context.LicenseFiles.Add(licenseFile);
        }

        public ICollection<License> GetByEmployee(int employeeId)
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Type)
                .Where(x => x.EmployeeId == employeeId).ToList();
        }

        public ICollection<License> GetByStatus(LicenseStatus statusId)
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Type)
                .Where(x => x.Status == statusId).ToList();
        }

        public ICollection<License> Search(LicenseSearchParams parameters)
        {
            var query = context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Type)
                .Where(x => x.Status == LicenseStatus.Approved);

            if (parameters.EmployeeId > 0)
                query = query.Where(x => x.EmployeeId == parameters.EmployeeId);

            if (parameters.LicenseTypeId > 0)
                query = query.Where(x => x.TypeId == parameters.LicenseTypeId);

            return query.ToList();
        }

        public ICollection<License> GetByManager(int managerId)
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Type)
                .Where(x => x.ManagerId == managerId).ToList();
        }

        public ICollection<License> GetByManagerAndStatus(LicenseStatus statusId, int managerId)
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Type)
                .Where(x => x.ManagerId == managerId && x.Status == statusId).ToList();
        }

        public ICollection<License> GetByEmployeeAndDates(int employeeId, DateTime startDate, DateTime endDate)
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Type)
                .Where(x => x.EmployeeId == employeeId && x.StartDate.Date >= startDate.Date && x.EndDate.Date <= endDate.Date).ToList();
        }

        public void UpdateStatus(License license)
        {
            context.Entry(license).Property("Status").IsModified = true;
        }

        public void AddHistory(LicenseHistory history)
        {
            context.LicenseHistories.Add(history);
        }

        public License GetById(int id)
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Type)
                .Include(x => x.Sector)
                .Include(x => x.LicenseFiles).ThenInclude(x => x.File)
                .SingleOrDefault(x => x.Id == id);
        }
    }
}
