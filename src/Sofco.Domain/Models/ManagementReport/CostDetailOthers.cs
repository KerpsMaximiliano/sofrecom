using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailOther : BaseEntity
    {
        public int CostDetailId { get; set; }

        public CostDetail CostDetail { get; set; }

        //public int CostDetailTypeId { get; set; }
        //public CostDetailType CostDetailType { get; set; }

        public int CostDetailSubtypeId { get; set; }
        public CostDetailSubtype CostDetailSubtype { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public decimal Value { get; set; }

        public string Description { get; set; }

        public bool IsReal { get; set; }
    }
}
