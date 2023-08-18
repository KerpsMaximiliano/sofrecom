using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AllocationManagement;
using System.Collections.Generic;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface ILicenseTypeRepository : IBaseRepository<LicenseType>
    {
        ICollection<LicenseType> GetAllActivesReadOnly();
    }
}
