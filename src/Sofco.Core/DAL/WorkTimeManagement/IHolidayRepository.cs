using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.DAL.WorkTimeManagement
{
    public interface IHolidayRepository : IBaseRepository<Holiday>
    {
        IList<Holiday> Get(int year);
    }
}