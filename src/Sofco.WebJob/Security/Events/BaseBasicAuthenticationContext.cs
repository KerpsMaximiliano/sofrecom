using System;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Sofco.WebJob.Security.Events
{
    public class BaseBasicAuthenticationContext : BaseControlContext
    {
        public BaseBasicAuthenticationContext(HttpContext context, BasicAuthenticationOptions options) : base(context)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Options = options;
        }

        public BasicAuthenticationOptions Options { get; }
    }
}
