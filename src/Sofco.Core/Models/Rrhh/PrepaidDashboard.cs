using System.Collections.Generic;

namespace Sofco.Core.Models.Rrhh
{
    public class PrepaidDashboard
    {
        public string Prepaid { get; set; }
        public int CountError { get; set; }
        public int CountSuccess { get; set; }
    }

    public class PrepaidResponse
    {
        public IList<PrepaidDashboard> Dashboard { get; set; }

        public bool MustSyncWithTiger { get; set; }
    }
}
