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
        ICollection<License> GetByEmployee(int employeeId);
        ICollection<License> GetByStatus(LicenseStatus statusId);
        ICollection<License> Search(LicenseSearchParams parameters);
        ICollection<License> GetByManager(int managerId);
        ICollection<License> GetByManagerAndStatus(LicenseStatus statusId, int managerId);
    }
}
