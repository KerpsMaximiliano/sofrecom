using System;
using System.Collections.Generic;
using Sofco.Domain;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeModel : BaseEntity
    {
        public IList<WorkTimeCalendarModel> Calendar { get; set; }

        public WorkTimeResumeModel Resume { get; set; }

        public IList<Holiday> Holidays { get; set; }

        public DateTime PeriodStartDate { get; set; }

        public DateTime PeriodEndDate { get; set; }
    }
}
