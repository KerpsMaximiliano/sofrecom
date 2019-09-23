using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
   public class CostDetailCategories : BaseEntity
    {
        public string Name { get; set; }
        public bool Default { get; set; }
        public bool BelongEmployee { get; set; }

        public ICollection<CostDetailSubcategories> Subcategories { get; set; }
    }
}
