using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Rrhh;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseService
    {
        Response<string> Add(License createDomain);
        Task<Response<File>> AttachFile(int id, Response<File> response, IFormFile file);
        IList<LicenseListItem> GetById(LicenseStatus statusId);
        IList<LicenseListItem> Search(LicenseSearchParams parameters);
        IList<LicenseListItem> GetByManager(int managerId);
        IList<LicenseListItem> GetByManagerAndStatus(LicenseStatus statusId, int managerId);
        IList<LicenseListItem> GetByEmployee(int employeeId);
    }
}
