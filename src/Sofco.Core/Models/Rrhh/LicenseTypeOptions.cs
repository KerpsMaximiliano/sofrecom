using System.Collections.Generic;
using Sofco.Domain.Utils;

namespace Sofco.Core.Models.Rrhh
{
    public class LicenseTypeOptions
    {
        public IList<Option> OptionsWithPayment { get; set; }

        public IList<Option> OptionsWithoutPayment { get; set; }
    }
}
