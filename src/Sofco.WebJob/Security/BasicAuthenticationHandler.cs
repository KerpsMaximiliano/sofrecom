using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Sofco.WebJob.Security.Events;

namespace Sofco.WebJob.Security
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private const string Scheme = "Basic";

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.Fail("No authorization header.");
            }

            if (!authorizationHeader.StartsWith(Scheme + ' ', StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Success(ticket: null);
            }

            string encodedCredentials = encodedCredentials = authorizationHeader.Substring(Scheme.Length).Trim();
            
            if (string.IsNullOrEmpty(encodedCredentials))
            {
                const string noCredentialsMessage = "No credentials";
                Logger.LogInformation(noCredentialsMessage);
                return AuthenticateResult.Fail(noCredentialsMessage);
            }

            try
            {
                string decodedCredentials;
                try
                {
                    decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to decode credentials : {encodedCredentials}", ex);
                }

                var delimiterIndex = decodedCredentials.IndexOf(':');
                if (delimiterIndex == -1)
                {
                    const string missingDelimiterMessage = "Invalid credentials, missing delimiter.";
                    Logger.LogInformation(missingDelimiterMessage);
                    return AuthenticateResult.Fail(missingDelimiterMessage);
                }

                var username = decodedCredentials.Substring(0, delimiterIndex);
                var password = decodedCredentials.Substring(delimiterIndex + 1);

                var validateCredentialsContext = new ValidateCredentialsContext(Context, Options)
                {
                    Username = username,
                    Password = password
                };

                await Options.Events.ValidateCredentials(validateCredentialsContext);

                if (validateCredentialsContext.Ticket != null)
                {
                    Logger.LogInformation($"Credentials validated for {username}");
                    return AuthenticateResult.Success(validateCredentialsContext.Ticket);
                }
                else
                {
                    Logger.LogInformation($"Credential validation failed for {username}");
                    return AuthenticateResult.Fail("Invalid credentials.");
                }
            }
            catch (Exception ex)
            {
                var authenticationFailedContext = new AuthenticationFailedContext(Context, Options)
                {
                    Exception = ex
                };

                await Options.Events.AuthenticationFailed(authenticationFailedContext);
                if (authenticationFailedContext.HandledResponse)
                {
                    return AuthenticateResult.Success(authenticationFailedContext.Ticket);
                }

                if (authenticationFailedContext.Skipped)
                {
                    return AuthenticateResult.Success(ticket: null);
                }

                throw;
            }
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            Response.StatusCode = 401;

            var headerValue = Scheme + $" realm=\"{Options.Realm}\"";
            Response.Headers.Append(HeaderNames.WWWAuthenticate, headerValue);

            return Task.FromResult(true);
        }

        protected override Task<bool> HandleForbiddenAsync(ChallengeContext context)
        {
            Response.StatusCode = 403;
            return Task.FromResult(true);
        }

        protected override Task HandleSignOutAsync(SignOutContext context)
        {
            throw new NotSupportedException();
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            throw new NotSupportedException();
        }
    }
}
