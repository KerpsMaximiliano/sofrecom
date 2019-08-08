using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.Common
{
    public class Delegation : BaseEntity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public DelegationType Type { get; set; }

        public int GrantedUserId { get; set; }

        public User GrantedUser { get; set; }

        public int? AnalyticSourceId { get; set; }

        public int? UserSourceId { get; set; }

        public DelegationSourceType SourceType { get; set; }

        public DateTime Created { get; set; }
    }
}
