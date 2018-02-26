using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Sofco.Common.Security.Interfaces;

namespace Sofco.Common.Security
{
    public class SessionManager : ISessionManager
    {
        private readonly IHttpContextAccessor contextAccessor;

        public SessionManager(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public string GetUserName()
        {
            if (contextAccessor?.HttpContext == null) return string.Empty;

            var identityName = contextAccessor.HttpContext.User.Identity.Name;

            return identityName.Split('@')[0];
        }
    }
}
