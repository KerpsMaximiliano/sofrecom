using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseTypeService
    {
        ICollection<LicenseType> GetOptions();
        Response UpdateLicenseTypeDays(int typeId, int value);
    }
}
