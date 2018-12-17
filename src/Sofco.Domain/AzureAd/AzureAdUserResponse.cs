using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sofco.Domain.AzureAd
{
    public class AzureAdUserResponse
    {
        public string DisplayName { get; set; }
        public string UserPrincipalName { get; set; }
        public string UserName => UserPrincipalName.Split('@')[0];
    }

    public class AzureAdUserListResponse
    {
        [JsonProperty("@odata.nextLink")]
        public string NextLink { get; set; }

        public AzureAdUserListResponse()
        {
            Value = new List<AzureAdUserResponse>();
        }

        public IList<AzureAdUserResponse> Value { get; set; }
    }
}
