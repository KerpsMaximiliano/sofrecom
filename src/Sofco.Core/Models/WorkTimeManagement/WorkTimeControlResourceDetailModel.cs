using System;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeControlResourceDetailModel
    {
        public string TaskDescription { get; set; }

        public string Reference { get; set; }

        public string CategoryDescription { get; set; }

        public decimal RegisteredHours { get; set; }

        public DateTime Date { get; set; }

        public string Comments { get; set; }
    }
}
