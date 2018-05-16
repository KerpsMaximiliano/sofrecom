using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.DAL.WorkTimeManagement
{
    public interface IHolidayRepository : IBaseRepository<Holiday>
    {
        List<Holiday> Get(int year);

        void Save(Holiday holiday);

        void SaveFromExternalData(List<Holiday> holidays);
    }
}