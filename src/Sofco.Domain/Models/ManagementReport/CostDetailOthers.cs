namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailOther : BaseEntity
    {
        public int CostDetailId { get; set; }

        public CostDetail CostDetail { get; set; }

        public int CostDetailTypeId { get; set; }

        public CostDetailType CostDetailType { get; set; }

        public decimal Value { get; set; }

        public string Description { get; set; }

        public bool IsReal { get; set; }
    }
}
