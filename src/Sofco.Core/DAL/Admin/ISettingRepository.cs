using System.Collections.Generic;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface ISettingRepository
    {
        List<Setting> GetAll();

        void Save(List<Setting> globalSettings);

        Setting GetByKey(string examdaysallowtogether);
        ICollection<Setting> GetHolidaysValues();
    }
}