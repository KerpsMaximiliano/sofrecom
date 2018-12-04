using System.Collections.Generic;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Domain.Utils
{
    public class AdvancementReturnForm : Option
    {
        public IList<Advancement> Advancements { get; set; }
    } 
}
