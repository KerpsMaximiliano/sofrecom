using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
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
        Response<string> Add(LicenseAddModel createDomain);
        Task<Response<File>> AttachFile(int id, Response<File> response, IFormFile file);
        IList<LicenseListItem> GetByStatus(LicenseStatus statusId);
        IList<LicenseListItem> Search(LicenseSearchParams parameters);
        IList<LicenseListItem> GetByManager(int managerId);
        IList<LicenseListItem> GetByManagerAndStatus(LicenseStatus statusId, int managerId);
        IList<LicenseListItem> GetByEmployee(int employeeId);
        Response DeleteFile(int id);
        Response ChangeStatus(int id, LicenseStatusChangeModel model, License license);
        Response<LicenseDetailModel> GetById(int id);
        ICollection<LicenseHistoryModel> GetHistories(int id);
        ExcelPackage GetLicenseReport();
        Response FileDelivered(int id);
    }
}
