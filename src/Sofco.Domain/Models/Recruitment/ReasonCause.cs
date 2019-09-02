using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Recruitment
{
    public class ReasonCause : Option
    {
        public ReasonCauseType Type { get; set; }
    }
}
