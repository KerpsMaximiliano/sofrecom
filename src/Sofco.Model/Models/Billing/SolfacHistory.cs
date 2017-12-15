using Sofco.Model.Enums;

namespace Sofco.Model.Models.Billing
{
    public class SolfacHistory : History
    {
        public SolfacStatus SolfacStatusFrom { get; set; }
        public SolfacStatus SolfacStatusTo { get; set; }

        public int SolfacId { get; set; }
        public Solfac Solfac { get; set; }
    }
}
