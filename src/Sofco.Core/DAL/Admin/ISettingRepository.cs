using System.Collections.Generic;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface ISettingRepository
    {
        List<Setting> GetAll();

        void Save(List<Setting> settings);

        void Save(Setting setting);

        Setting GetByKey(string examdaysallowtogether);

        ICollection<Setting> GetHolidaysValues();
    }
}