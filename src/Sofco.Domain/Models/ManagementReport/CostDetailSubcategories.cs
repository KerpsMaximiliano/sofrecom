using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailSubcategories : BaseEntity
    {
        public string Name { get; set; }

        public int CostDetailCategoryId { get; set; }
        public CostDetailCategories CostDetailCategory { get; set; }

        public string Codigo { get; set; }

        public ICollection<CostDetailStaff> CostDetailStaff { get; set; }
        public ICollection<CostDetailOther> CostDetailOther { get; set; }

    }
}
