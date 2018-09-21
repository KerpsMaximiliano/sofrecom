using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseService
    {
        Response<string> Add(LicenseAddModel createDomain);
        Task<Response<File>> AttachFile(int id, Response<File> response, IFormFile file);
        IList<LicenseListModel> GetByStatus(LicenseStatus statusId);
        IList<LicenseListModel> Search(LicenseSearchParams parameters);
        IList<LicenseListModel> GetByManager(int managerId);
        IList<LicenseListModel> GetByManagerAndStatus(LicenseStatus statusId, int managerId);
        IList<LicenseListModel> GetByEmployee(int employeeId);
        Response DeleteFile(int id);
        Response ChangeStatus(int id, LicenseStatusChangeModel model, License license);
        Response<LicenseDetailModel> GetById(int id);
        ICollection<LicenseHistoryModel> GetHistories(int id);
        Response<byte[]> GetLicenseReport(ReportParams parameters);
        Response FileDelivered(int id);
    }
}
