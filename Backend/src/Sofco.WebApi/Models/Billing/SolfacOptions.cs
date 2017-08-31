using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacOptions
    {
        public IList<SelectListItem> Provinces { get; set; }
        public IList<SelectListItem> DocumentTypes { get; set; }
        public IList<SelectListItem> ImputationNumbers { get; set; }
        public IList<SelectListItem> Currencies { get; set; }
    }
}
