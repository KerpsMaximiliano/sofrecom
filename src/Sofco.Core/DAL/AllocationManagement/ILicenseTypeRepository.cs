using System.Collections.Generic;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface ILicenseTypeRepository
    {
        void Save(List<LicenseType> licenseTypes);
    }
}
