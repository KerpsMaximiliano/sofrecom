using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseTypeService
    {
        ICollection<LicenseType> GetOptions();
    }
}
