using System.Collections.Generic;
using Sofco.Core.Models.Rrhh;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface ISettingService
    {
        Response<List<GlobalSetting>> GetAll();

        Response<List<GlobalSetting>> Save(List<GlobalSetting> globalSettings);

        IList<LicenseTypeSettingItem> GetLicenseTypes();
    }
}