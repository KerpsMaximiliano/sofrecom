using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Model.Utils;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacOptions
    {
        public IList<Option> Provinces { get; set; }

        public IList<Option> DocumentTypes { get; set; }

        public IList<Option> ImputationNumbers { get; set; }

        public IList<Option> Currencies { get; set; }

        public IList<Option> PaymentTerms { get; set; }
    }
}
