using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Model.Relationships;

namespace Sofco.Model.Models.Admin
{
    public class Category : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IList<Task> Tasks { get; set; }

        public ICollection<EmployeeCategory> EmployeeCategories { get; set; }
    }
}
