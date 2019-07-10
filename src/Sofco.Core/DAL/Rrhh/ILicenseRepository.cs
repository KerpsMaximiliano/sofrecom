using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Relationships;

namespace Sofco.Core.DAL.Rrhh
{
    public interface ILicenseRepository : IBaseRepository<License>
    {
        void AddFile(LicenseFile licenseFile);
        IList<License> GetLicensesLastMonth();
        ICollection<License> GetByEmployee(int employeeId);
        ICollection<License> GetByEmployee(List<int> employeeIds);
        ICollection<License> GetByEmployeeAndStatus(List<int> employeeIds, LicenseStatus statusId);
        ICollection<License> GetByStatus(LicenseStatus statusId);
        ICollection<License> Search(LicenseSearchParams parameters);
        ICollection<License> GetByManager(int managerId);
        ICollection<License> GetByManagerIds(List<int> managerIds);
        ICollection<License> GetByManagerAndStatus(LicenseStatus statusId, int managerId);
        ICollection<License> GetByEmployeeAndDates(int employeeId, DateTime startDate, DateTime endDate);
        ICollection<License> GetByEmployeeAndStartDate(int employeeId, DateTime startDate);
        void UpdateStatus(License license);
        void AddHistory(LicenseHistory history);
        License GetById(int id);
        ICollection<LicenseHistory> GetHistories(int id);

        List<License> GetPendingCertificates();
        IList<License> GetLicensesReport(ReportParams parameters);
        bool AreDatesOverlaped(DateTime startDate, DateTime endDate, int employeeId);
        ICollection<License> GetByDateAndType(DateTime date, int typeLicense);
    }
}
