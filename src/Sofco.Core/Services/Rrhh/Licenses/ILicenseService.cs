using System.Collections.Generic;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Rrhh.Licenses
{
    public interface ILicenseService
    {
        Response<string> Add(LicenseAddModel createDomain);
        IList<LicenseListModel> GetByStatus(LicenseStatus statusId);
        IList<LicenseListModel> Search(LicenseSearchParams parameters);
        IList<LicenseListModel> GetByManager(int managerId);
        IList<LicenseListModel> GetByManagerAndStatus(LicenseStatus statusId, int managerId);
        IList<LicenseListModel> GetByEmployee(int employeeId);
        Response ChangeStatus(int id, LicenseStatusChangeModel model, License license);
        Response<LicenseDetailModel> GetById(int id);
        ICollection<LicenseHistoryModel> GetHistories(int id);
    }
}
