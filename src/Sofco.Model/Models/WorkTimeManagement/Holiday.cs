using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.WorkTimeManagement
{
    public class Holiday : BaseEntity, IEntityDate
    {
        public DateTime Date { get; set; }

        public string Name { get; set; }

        public string DataSource { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
