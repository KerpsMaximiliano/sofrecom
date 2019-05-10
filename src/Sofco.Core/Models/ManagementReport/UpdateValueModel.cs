using Sofco.Domain.Enums;

namespace Sofco.Core.Models.ManagementReport
{
    public class UpdateValueModel
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public EvalPropType Type { get; set; }
    }
}
