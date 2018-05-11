﻿using System.Collections.Generic;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface ISettingRepository
    {
        List<Setting> GetAll();

        void Save(List<Setting> settings);

        void Save(Setting setting);

        Setting GetByKey(string key);

        ICollection<Setting> GetHolidaysValues();

        List<Setting> GetByCategory(SettingCategory category);
    }
}