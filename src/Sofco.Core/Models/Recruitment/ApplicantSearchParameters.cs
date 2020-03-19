using System.Collections.Generic;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantSearchParameters
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IList<int> Skills { get; set; }

        public IList<int> Profiles { get; set; }

        public IList<int> StatusIds { get; set; }
    }
}
