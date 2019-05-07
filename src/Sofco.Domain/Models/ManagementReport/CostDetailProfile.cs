namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailProfile : BaseEntity
    {
        public int CostDetailId { get; set; }

        public CostDetail CostDetail { get; set; }

        public decimal Value { get; set; }

        public string Description { get; set; }
    }
}
