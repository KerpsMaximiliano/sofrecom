using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Core.Models.Rrhh;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Services.Admin
{
    public interface ISettingService
    {
        Result<List<GlobalSetting>> GetAll();

        Result Save(List<GlobalSetting> globalSettings);

        IList<LicenseTypeSettingItem> GetLicenseTypes();
    }
}