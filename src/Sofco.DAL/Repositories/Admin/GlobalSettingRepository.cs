using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Admin;

namespace Sofco.DAL.Repositories.Admin
{
    public class GlobalSettingRepository : BaseRepository<GlobalSetting>, IGlobalSettingRepository
    {
        public GlobalSettingRepository(SofcoContext context) : base(context)
        {
        }

        public new List<GlobalSetting> GetAll()
        {
            return context.GlobalSettings.ToList();
        }

        public void Save(List<GlobalSetting> globalSettings)
        {
            var items = context.GlobalSettings.Where(s => globalSettings.Select(x => x.Id).Contains(s.Id));

            foreach (var item in items)
            {
                var data = globalSettings.Single(s => s.Id == item.Id);

                Update(item, data);
            }

            context.SaveChanges();
        }

        public GlobalSetting GetByKey(string examdaysallowtogether)
        {
            return context.GlobalSettings.SingleOrDefault(x => x.Key.Equals(examdaysallowtogether));
        }

        private void Update(GlobalSetting storedData, GlobalSetting data)
        {
            storedData.Value = data.Value;

            storedData.Modified = DateTime.UtcNow;

            Update(storedData);
        }
    }
}
