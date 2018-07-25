using System.Collections.Generic;
using Sofco.Core.Models.Rrhh;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseViewDelegateService
    {
        Response<List<LicenseViewDelegateModel>> GetAll();

        Response<UserDelegate> Save(UserDelegate userDelegate);

        Response Delete(int userDeletegateId);
    }
}