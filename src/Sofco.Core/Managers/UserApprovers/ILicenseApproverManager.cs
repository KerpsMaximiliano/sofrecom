using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.Managers.UserApprovers
{
    public interface ILicenseApproverManager
    {
        List<License> GetByCurrent();

        List<License> GetByCurrentByStatus(LicenseStatus statusId);
    }
}