using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface ILicenseTypeRepository
    {
        void Save(List<LicenseType> licenseTypes);
    }
}
