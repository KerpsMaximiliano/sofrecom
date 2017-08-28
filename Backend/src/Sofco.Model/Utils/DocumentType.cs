using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Utils
{
    public class DocumentType : Option
    {
        public IList<Solfac> Solfacs { get; set; }
    }
}
