using System;
using System.Collections.Generic;
using Sofco.Model;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeModel : BaseEntity
    {
        public IList<WorkTimeCalendarModel> Calendar { get; set; }

        public WorkTimeResumeModel Resume { get; set; }
    }
}
