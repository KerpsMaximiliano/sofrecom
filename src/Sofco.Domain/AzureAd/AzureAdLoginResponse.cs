﻿namespace Sofco.Domain.AzureAd
{
    public class AzureAdLoginResponse
    {
        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public int expires_in { get; set; }
    }
}
