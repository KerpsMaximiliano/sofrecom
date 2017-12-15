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

        public string ClientSecret { get; set; }

        public string ClientCredentials { get; set; }

        public string Scope { get; set; }

        public string ClientSecretId { get; set; }

        public string GraphUsersUrl { get; set; }

        public string GraphTokenUrl { get; set; }
    }
}
