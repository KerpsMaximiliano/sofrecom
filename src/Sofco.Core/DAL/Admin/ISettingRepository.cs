using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface ISettingRepository : IBaseRepository<Setting>
    {
        new List<Setting> GetAll();

        void Save(List<Setting> settings);

        void Save(Setting setting);

        Setting GetByKey(string key);

        ICollection<Setting> GetHolidaysValues();

        List<Setting> GetByCategory(SettingCategory category);
    }
}