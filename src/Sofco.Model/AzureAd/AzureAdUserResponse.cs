using System.Collections.Generic;

namespace Sofco.Model.AzureAd
{
    public class AzureAdUserResponse
    {
        public string DisplayName { get; set; }
        public string UserPrincipalName { get; set; }
        public string UserName
        {
            get
            {
                return UserPrincipalName.Split('@')[0];
            }
        }
    }

    public class AzureAdUserListResponse
    {
        public AzureAdUserListResponse()
        {
            Value = new List<AzureAdUserResponse>();
        }

        public IList<AzureAdUserResponse> Value { get; set; }
    }
}
