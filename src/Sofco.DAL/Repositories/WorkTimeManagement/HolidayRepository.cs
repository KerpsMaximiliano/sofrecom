using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.DAL.Repositories.WorkTimeManagement
{
    public class HolidayRepository : BaseRepository<Holiday>, IHolidayRepository
    {
        public HolidayRepository(SofcoContext context) : base(context)
        {
        }

        public List<Holiday> Get(int year)
        {
            return context.Holidays.Where(s => s.Date.Year == year).ToList();
        }

        public void Save(Holiday holiday)
        {
            if (holiday.Id == 0)
            {
                Insert(holiday);
                return;
            }

            Update(holiday);

            context.SaveChanges();
        }

        private new void Update(Holiday holiday)
        {
            var stored = context.Holidays.SingleOrDefault(x => x.Id == holiday.Id);

            if (stored == null) throw new Exception("Item Not Found");

            stored.Date = holiday.Date;
            stored.DataSource = holiday.DataSource;
            stored.Name = holiday.Name;
        }
    }
}
