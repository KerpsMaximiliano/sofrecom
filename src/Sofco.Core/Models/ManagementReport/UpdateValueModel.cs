using Sofco.Domain.Enums;

namespace Sofco.Core.Models.ManagementReport
{
    public class UpdateValueModel
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public EvalPropType Type { get; set; }
    }

    public class UpdateBillingDataModel
    {
        public int Id { get; set; }

        public decimal? EvalPropDifference { get; set; }

        public int? Resources { get; set; }

        public string Comments { get; set; }

        public ReportBillingUpdateDataType Type { get; set; }
    }
}
