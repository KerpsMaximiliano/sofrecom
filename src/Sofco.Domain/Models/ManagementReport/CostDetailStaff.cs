namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailStaff : BaseEntity
    {
        public decimal Value { get; set; }
        public string Description { get; set; }

        public int CostDetailId { get; set; }
        public CostDetail CostDetail { get; set; }

        public int CostDetailSubcategoryId { get; set; }
        public CostDetailSubcategories CostDetailSubcategory { get; set; }

        public int BudgetTypeId { get; set; }
        public BudgetType BudgetType { get; set; }

    }
}
