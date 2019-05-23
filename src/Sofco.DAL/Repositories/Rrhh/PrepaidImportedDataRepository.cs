using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.Models.Rrhh;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Rrhh;

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
                .Where(x => x.Date.Date == date.Date)
                .GroupBy(x => x.Prepaid)
                .Select(x => new Tuple<string, IList<PrepaidImportedData>>(x.Key, x.ToList()))
                .ToList();

            var list = new List<PrepaidDashboard>();

            foreach (var tuple in data)
            {
                var item = new PrepaidDashboard();

                item.Prepaid = tuple.Item1;
                item.CountError = tuple.Item2.Count(x => x.Status == PrepaidImportedDataStatus.Error);
                item.CountSuccess = tuple.Item2.Count(x => x.Status == PrepaidImportedDataStatus.Success);

                list.Add(item);
            }

            return list;
        }
    }
}
