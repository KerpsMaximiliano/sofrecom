using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
   public class CostDetailCategories : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<CostDetailSubcategories> CostDetailSubcategories { get; set; }
    }
}
