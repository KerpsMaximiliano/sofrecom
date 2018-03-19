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
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public BasicAuthenticationOptions Options { get; }
    }
}
