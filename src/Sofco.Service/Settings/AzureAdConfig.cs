namespace Sofco.Service.Settings
{
    public class AzureAdConfig
    {
        public string ClientId { get; set; }

        public string Tenant { get; set; }

        public string AadInstance { get; set; }

        public string Domain { get; set; }

        public string Audience { get; set; }

        public string GrantType { get; set; }
    }
}
