using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;

namespace Sofco.Common.Security
{
    public class SessionManager : ISessionManager
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly string currentDomain;

        public SessionManager(IHttpContextAccessor contextAccessor, IOptions<AppSetting> appOptions)
        {
            this.contextAccessor = contextAccessor;
            currentDomain = appOptions.Value.Domain;
        }

        public string GetUserName()
        {
            if (contextAccessor?.HttpContext == null) return string.Empty;

            var identityName = contextAccessor.HttpContext.User.Identity.Name;

            return identityName.Split('@')[0];
        }

        public string GetUserMail()
        {
            return $"{GetUserName()}@{currentDomain}";
        }

        public string GetUserEmail(string text)
        {
            return text.IndexOf('@') > -1 ? text : $"{text}@{currentDomain}";
        }
    }
}
