using System.Collections.Generic;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchModel
    {
        public int UserId { get; set; }

        public int RecruiterId { get; set; }

        public int ReasonCauseId { get; set; }

        public string ClientCrmId { get; set; }

        public int ClientId { get; set; }

        public IList<int> Profiles { get; set; }

        public IList<int> Skills { get; set; }

        public IList<int> Seniorities { get; set; }

        public int Quantity { get; set; }

        public string TimeHiring { get; set; }

        public decimal MaximunSalary { get; set; }

        public string Comments { get; set; }
    }
}
