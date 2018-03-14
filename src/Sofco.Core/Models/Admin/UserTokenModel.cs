namespace Sofco.Core.Models.Admin
{
    public class UserTokenModel
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresIn { get; set; }
    }
}
