using System.Collections.Generic;
using Sofco.Model;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeModel : BaseEntity
    {
        public IList<WorkTimeCalendarModel> Calendar { get; set; }

        public WorkTimeResumeModel Resume { get; set; }

        public IList<Holiday> Holidays { get; set; }
    }
}
