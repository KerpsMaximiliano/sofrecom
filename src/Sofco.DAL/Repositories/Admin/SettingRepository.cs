using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class SettingRepository : BaseRepository<Setting>, ISettingRepository
    {
        public SettingRepository(SofcoContext context) : base(context)
        {
        }

        public new List<Setting> GetAll()
        {
            return context.Settings.ToList();
        }

        public void Save(List<Setting> globalSettings)
        {
            var items = context.Settings.Where(s => globalSettings.Select(x => x.Id).Contains(s.Id));

            foreach (var item in items)
            {
                var data = globalSettings.Single(s => s.Id == item.Id);

                Update(item, data);
            }

            context.SaveChanges();
        }

        public void Save(Setting setting)
        {
            var item = context.Settings.SingleOrDefault(s => s.Id == setting.Id);

            if (item == null) return;

            Update(item, setting);

            context.SaveChanges();
        }

        public Setting GetByKey(string examdaysallowtogether)
        {
            return context.Settings.SingleOrDefault(x => x.Key.Equals(examdaysallowtogether));
        }

        public ICollection<Setting> GetHolidaysValues()
        {
            return context.Settings.Where(x => x.Key.Contains("Holidays")).ToList();
        }

        public List<Setting> GetByCategory(SettingCategory category)
        {
            return context.Settings.Where(x => x.Category == category).ToList();
        }

        private void Update(Setting storedData, Setting data)
        {
            storedData.Value = data.Value;

            storedData.Modified = DateTime.UtcNow;

            Update(storedData);
        }
    }
}
