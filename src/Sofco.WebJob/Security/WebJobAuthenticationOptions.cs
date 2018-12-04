//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http.Authentication;
//using Sofco.WebJob.Security.Events;

//namespace Sofco.WebJob.Security
//{
//    public class WebJobAuthenticationOptions
//    {
//        public static BasicAuthenticationOptions Config(string username, string password)
//        {
//            return new BasicAuthenticationOptions
//            {
//                Events = new BasicAuthenticationEvents
//                {
//                    OnValidateCredentials = context =>
//                    {
//                        if (context.Username == username
//                            && context.Password == password)
//                        {
//                            var claims = new[]
//                            {
//                                new Claim(ClaimTypes.NameIdentifier, context.Username)
//                            };
//                            context.Ticket = new AuthenticationTicket(
//                                new ClaimsPrincipal(
//                                    new ClaimsIdentity(claims, context.Options.AuthenticationScheme)),
//                                new AuthenticationProperties(),
//                                context.Options.AuthenticationScheme);
//                        }

//                        return Task.FromResult<object>(null);
//                    }
//                }
//            };
//        }
//    }
//}
