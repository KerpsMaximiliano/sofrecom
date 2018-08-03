using System.Collections.Generic;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface ISettingService
    {
        Response<List<Setting>> GetAll();

        Response<List<Setting>> Save(List<Setting> settings);

        Response<Setting> Save(Setting setting);

        IList<LicenseTypeSettingItem> GetLicenseTypes();

        Response<List<Setting>> GetJobSettings();
    }
}