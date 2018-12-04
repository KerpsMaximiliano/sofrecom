//using System;
//using Microsoft.AspNetCore.Builder;

//namespace Sofco.WebJob.Security
//{
//    /// <summary>
//    /// Extension methods to add Basic authentication capabilities to an HTTP application pipeline.
//    /// </summary>
//    public static class BasicAuthenticationAppBuilderExtensions
//    {
//        /// <summary>
//        /// Adds the <see cref="BasicAuthenticationMiddleware"/>to the specified <see cref="IApplicationBuilder"/>, which enables basic authentication capabilities.
//        /// </summary>
//        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
//        /// <returns>A reference to this instance after the operation has completed.</returns>
//        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app)
//        {
//            if (app == null)
//            {
//                throw new ArgumentNullException(nameof(app));
//            }

//            return app.UseMiddleware<BasicAuthenticationMiddleware>();
//        }

//        /// <summary>
//        /// Adds the <see cref="BasicAuthenticationMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>, which enables basic authentication capabilities.
//        /// </summary>
//        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
//        /// <param name="options">A <see cref="BasicAuthenticationOptions"/> that specifies options for the middleware.</param>
//        /// <returns>A reference to this instance after the operation has completed.</returns
//        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, BasicAuthenticationOptions options)
//        {
//            if (app == null)
//            {
//                throw new ArgumentNullException(nameof(app));
//            }

//            if (options == null)
//            {
//                throw new ArgumentNullException(nameof(options));
//            }

//            return app.UseMiddleware<BasicAuthenticationMiddleware>(options);
//        }
//    }
//}
