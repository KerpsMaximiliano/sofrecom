using System.Collections.Generic;

namespace Sofco.Core.Models.Common
{
    public class AnalyticsWithEmployees
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public IList<ResourceOption> Resources { get; set; }

        public string AccountId { get; set; }
    }

    public class ResourceOption
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int UserId { get; set; }
    }
}
