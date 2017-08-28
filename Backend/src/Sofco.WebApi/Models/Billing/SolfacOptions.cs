using System.Collections.Generic;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacOptions
    {
        public IList<Option<int>> Provinces { get; set; }
        public IList<Option<int>> DocumentTypes { get; set; }
        public IList<Option<int>> ImputationNumbers { get; set; }
        public IList<Option<int>> Currencies { get; set; }
    }
}
