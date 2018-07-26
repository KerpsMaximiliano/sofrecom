using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.Models.Rrhh;
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

        public IList<License> GetLicensesLastMonth()
        {
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var last = month.AddDays(-1).Month;

            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Type)
                .Where(x => x.StartDate.Month == last || x.EndDate.Month == last)
                .ToList();
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
                .Where(x => x.Status == LicenseStatus.Approved || x.Status == LicenseStatus.ApprovePending);

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

        public ICollection<License> GetByManagerIds(List<int> managerIds)
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Manager)
                .Include(x => x.Type)
                .Where(x => managerIds.Contains(x.ManagerId)).ToList();
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

        public ICollection<LicenseHistory> GetHistories(int id)
        {
            return context.LicenseHistories.Where(x => x.LicenseId == id).Include(x => x.User).ToList().AsReadOnly();
        }

        public List<License> GetPendingCertificates()
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Type)
                .Where(s => !s.HasCertificate && s.Status == LicenseStatus.ApprovePending)
                .ToList();
        }

        public IList<License> GetLicensesReport(ReportParams parameters)
        {
            return context.Licenses
                .Include(x => x.Employee)
                .Include(x => x.Type)
                .Where(x => x.StartDate.Date >= parameters.StartDate.Date && x.StartDate.Date <= parameters.EndDate.Date)
                .ToList();
        }

        public bool AreDatesOverlaped(DateTime startDate, DateTime endDate, int employeeId)
        {
            return context.Licenses.Any(x => x.EmployeeId == employeeId &&
                                             x.Status != LicenseStatus.Cancelled && x.Status != LicenseStatus.Rejected &&
                                             (startDate >= x.StartDate.Date && startDate <= x.EndDate.Date ||
                                              endDate >= x.StartDate.Date && endDate <= x.EndDate.Date));
        }
    }
}
