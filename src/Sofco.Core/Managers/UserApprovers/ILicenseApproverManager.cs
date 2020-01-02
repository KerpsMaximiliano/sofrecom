using System.Collections.Generic;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.Managers.UserApprovers
{
    public interface ILicenseApproverManager
    {
        List<License> GetByCurrent();

        List<License> GetByCurrentByStatus(LicenseStatus statusId);

        List<LicenseListModel> ResolveApprovers(List<LicenseListModel> models);

        List<string> GetEmailApproversByEmployeeId(int employeeId);

        bool HasUserAuthorizer();
    }
}