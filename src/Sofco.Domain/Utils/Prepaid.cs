using System.Collections.Generic;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Domain.Utils
{
    public class Prepaid : Option
    {
        public string Code { get; set; }

        public IList<PrepaidImportedData> PrepaidImportedData { get; set; }
    }
}
