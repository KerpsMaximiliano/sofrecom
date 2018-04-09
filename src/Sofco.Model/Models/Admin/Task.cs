using System;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.Admin
{
    public class Task : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
