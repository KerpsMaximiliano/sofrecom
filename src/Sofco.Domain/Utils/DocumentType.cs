using System.Collections.Generic;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Utils
{
    public class DocumentType : Option
    {
        public IList<Solfac> Solfacs { get; set; }
    }
}
