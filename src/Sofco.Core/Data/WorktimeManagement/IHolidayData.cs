using System.Collections.Generic;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Data.WorktimeManagement
{
    public interface IHolidayData
    {
        List<Holiday> Get(int year, int month);
    }
}