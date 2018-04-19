using Sofco.Model;

namespace Sofco.Core.Models.AllocationManagement
{
    public class OptionPercentage : BaseEntity
    {
        public string Text { get; set; }
        public decimal StartValue { get; set; }
        public decimal EndValue { get; set; }
    }
}
