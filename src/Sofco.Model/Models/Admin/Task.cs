using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Domain.Models.Admin
{
    public class Task : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ICollection<WorkTime> WorkTimes { get; set; }
    }
}
