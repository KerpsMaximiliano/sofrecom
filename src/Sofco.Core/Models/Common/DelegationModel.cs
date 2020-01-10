using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.Models.Common
{
    public class DelegationModel
    {
        public DelegationModel(Delegation userDelegate)
        {
            Id = userDelegate.Id;
            Type = userDelegate.Type;
            TypeDescription = userDelegate.Type.ToString();
            GrantedUserId = userDelegate.GrantedUserId;
            GrantedUserName = userDelegate.GrantedUser?.Name;
            AnalyticSourceId = userDelegate.AnalyticSourceId;
            UserSourceId = userDelegate.UserSourceId;
            Created = userDelegate.Created;
        }

        public int Id { get; set; }

        public string TypeDescription { get; set; }

        public string GrantedUserName { get; set; }

        public DateTime Created { get; set; }

        public string AnalyticSourceName { get; set; }

        public string UserSourceName { get; set; }

        public DelegationType? Type { get; set; }

        public int? AnalyticSourceId { get; set; }

        public int? UserSourceId { get; set; }

        public int GrantedUserId { get; set; }
    }

    public class DelegationAddModel
    {
        public DelegationType? Type { get; set; }

        public DelegationSourceType SourceType { get; set; }

        public int GrantedUserId { get; set; }

        public int? AnalyticSourceId { get; set; }

        public IList<int> UserSourceIds { get; set; }
    }
}
