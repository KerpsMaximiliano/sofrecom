using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Rrhh
{
    public interface ILicenseRepository : IBaseRepository<License>
    {
        void AddFile(LicenseFile licenseFile);
        IList<License> GetLicensesLastMonth();
        ICollection<License> GetByEmployee(int employeeId);
        ICollection<License> GetByStatus(LicenseStatus statusId);
        ICollection<License> Search(LicenseSearchParams parameters);
        ICollection<License> GetByManager(int managerId);
        ICollection<License> GetByManagerAndStatus(LicenseStatus statusId, int managerId);
        ICollection<License> GetByEmployeeAndDates(int employeeId, DateTime startDate, DateTime endDate);
        void UpdateStatus(License license);
        void AddHistory(LicenseHistory history);
        License GetById(int id);
        ICollection<LicenseHistory> GetHistories(int id);
    }
}
