using System.Collections.Generic;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface IGlobalSettingRepository
    {
        List<GlobalSetting> GetAll();

        void Save(List<GlobalSetting> globalSettings);
    }
}