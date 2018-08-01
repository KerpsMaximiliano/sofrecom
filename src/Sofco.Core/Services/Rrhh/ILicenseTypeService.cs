using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseTypeService
    {
        ICollection<LicenseType> GetOptions();
        Response UpdateLicenseTypeDays(int typeId, int value);
    }
}
