using System.Collections.Generic;
using Sofco.Model.Utils;

namespace Sofco.WebApi.Models.Rrhh
{
    public class LicenseTypeOptions
    {
        public IList<Option> OptionsWithPayment { get; set; }

        public IList<Option> OptionsWithoutPayment { get; set; }
    }
}
