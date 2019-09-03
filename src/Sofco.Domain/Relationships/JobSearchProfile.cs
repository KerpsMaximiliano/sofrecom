using Sofco.Domain.Models.Recruitment;

namespace Sofco.Domain.Relationships
{
    public class JobSearchProfile
    {
        public int JobSearchId { get; set; }
        public JobSearch JobSearch { get; set; }

        public int ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}
