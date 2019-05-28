using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.Models.Rrhh;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.DAL.Repositories.Rrhh
{
    public class PrepaidImportedDataRepository : BaseRepository<PrepaidImportedData>, IPrepaidImportedDataRepository
    {
        public PrepaidImportedDataRepository(SofcoContext context) : base(context)
        {
        }

        public IList<PrepaidDashboard> GetDashboard(int yearId, int monthId)
        {
            var date = new DateTime(yearId, monthId, 1);

            var data = context.PrepaidImportedData
                .Include(x => x.Prepaid)
                .Where(x => x.Date.Date == date.Date)
                .GroupBy(x => x.Prepaid)
                .Select(x => new Tuple<Prepaid, IList<PrepaidImportedData>>(x.Key, x.ToList()))
                .ToList();

            var list = new List<PrepaidDashboard>();

            foreach (var tuple in data)
            {
                var item = new PrepaidDashboard();

                item.Prepaid = tuple.Item1.Text;
                item.CountError = tuple.Item2.Count(x => x.Status == PrepaidImportedDataStatus.Error);
                item.CountSuccess = tuple.Item2.Count(x => x.Status == PrepaidImportedDataStatus.Success);

                list.Add(item);
            }

            return list;
        }

        public IList<PrepaidImportedData> GetByDate(int yearId, int monthId)
        {
            var date = new DateTime(yearId, monthId, 1);

            return context.PrepaidImportedData
                .Include(x => x.Prepaid)
                .Where(x => x.Date.Date == date.Date)
                .ToList()
                .AsReadOnly();
        }

        public IList<PrepaidImportedData> GetByIds(IList<int> modelIds)
        {
            return context.PrepaidImportedData.Where(x => modelIds.Contains(x.Id)).ToList();
        }

        public void UpdateStatus(PrepaidImportedData prepaidImportedData)
        {
            context.Entry(prepaidImportedData).Property("Status").IsModified = true;
        }

        public void DeleteByDateAndPrepaid(int prepaidId, DateTime dateTime)
        {
            var itemsToDelete = context.PrepaidImportedData.Where(x => x.PrepaidId == prepaidId && x.Period.Date == dateTime.Date).ToList();

            Delete(itemsToDelete);

            context.SaveChanges();
        }

        public void Close(PrepaidImportedData prepaidImportedData)
        {
            context.Entry(prepaidImportedData).Property("Closed").IsModified = true;
        }
    }
}
