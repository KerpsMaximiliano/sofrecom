using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Rrhh
{
    public interface ILicenseRepository : IBaseRepository<License>
    {
        void AddFile(LicenseFile licenseFile);
        ICollection<License> GetByEmployee(int employeeId);
    }
}
