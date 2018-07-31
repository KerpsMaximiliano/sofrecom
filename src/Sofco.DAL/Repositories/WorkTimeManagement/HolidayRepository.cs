using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public new List<Holiday> Get(int year)
        {
            return context.Holidays.Where(s => s.Date.ToUniversalTime().Year == year).OrderBy(s => s.Date).ToList();
        }

        public List<Holiday> Get(int year, int month)
        {
            return context.Holidays.Where(s => s.Date.ToUniversalTime().Year == year && s.Date.ToUniversalTime().Month == month).OrderBy(s => s.Date).ToList();
        }

        public void Save(Holiday holiday)
        {
            if (holiday.Id == 0)
            {
                Insert(holiday);

                context.SaveChanges();
                return;
            }

            Update(holiday);

            context.SaveChanges();
        }

        public void SaveFromExternalData(List<Holiday> holidays)
        {
            foreach (var holiday in holidays)
            {
                SaveFromExternalData(holiday);
            }
        }

        public void Delete(int holidayId)
        {
            var item = new Holiday { Id = holidayId };

            context.Entry(item).State = EntityState.Deleted;

            context.SaveChanges();
        }

        private void SaveFromExternalData(Holiday holiday)
        {
            var stored = GetStored(holiday);

            if (stored == null)
            {
                Insert(holiday);

                context.SaveChanges();

                return;
            }

            stored.Modified = DateTime.UtcNow;

            context.SaveChanges();
        }

        private Holiday GetStored(Holiday data)
        {
            return context.Holidays.FirstOrDefault(x =>
                x.Name == data.Name
                && x.Date.Month == data.Date.Month
                && x.Date.Year == data.Date.Year);
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
