using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Sofco.WebJob.Security.Events;

namespace Sofco.WebJob.Security
{
    /// <summary>
    /// Contains the options used by the BasicAuthenticationMiddleware
    /// </summary>
    public class BasicAuthenticationOptions : AuthenticationOptions, IOptions<BasicAuthenticationOptions>
    {
        private string realm;

        /// <summary>
        /// Create an instance of the options initialized with the default values
        /// </summary>
        public BasicAuthenticationOptions() : base()
        {
            AuthenticationScheme = BasicAuthenticationDefaults.AuthenticationScheme;
            AutomaticAuthenticate = true;
            AutomaticChallenge = true;
        }

        /// <summary>
        /// Gets or sets the Realm sent in the WWW-Authenticate header.
        /// </summary>
        /// <remarks>
        /// The realm value (case-sensitive), in combination with the canonical root URL 
        /// of the server being accessed, defines the protection space. 
        /// These realms allow the protected resources on a server to be partitioned into a 
        /// set of protection spaces, each with its own authentication scheme and/or 
        /// authorization database. 
        /// </remarks>
        public string Realm
        {
            get => realm;

            set
            {
                if (!IsAscii(value))
                {
                    throw new ArgumentOutOfRangeException("Realm", "Realm must be US ASCII");
                }

                realm = value;
            }
        }

        /// <summary>
        /// The object provided by the application to process events raised by the basic authentication middleware.
        /// The application may implement the interface fully, or it may create an instance of BasicAuthenticationEvents
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public IBasicAuthenticationEvents Events { get; set; } = new BasicAuthenticationEvents();

        BasicAuthenticationOptions IOptions<BasicAuthenticationOptions>.Value => this;

        private bool IsAscii(string input)
        {
            foreach (var c in input)
            {
                if (c < 32 || c >= 127)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
