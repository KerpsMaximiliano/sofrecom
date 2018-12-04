//using System;
//using System.Threading.Tasks;

//namespace Sofco.WebJob.Security.Events
//{
//    /// <summary>
//    /// This default implementation of the IBasicAuthenticationEvents may be used if the 
//    /// application only needs to override a few of the interface methods. 
//    /// This may be used as a base class or may be instantiated directly.
//    /// </summary>
//    public class BasicAuthenticationEvents : IBasicAuthenticationEvents
//    {
//        /// <summary>
//        /// A delegate assigned to this property will be invoked when the authentication fails.
//        /// </summary>
//        public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = context => Task.FromResult(0);

//        /// <summary>
//        /// A delegate assigned to this property will be invoked when the credentials need validation.
//        /// </summary>
//        /// <remarks>
//        /// You must provide a delegate for this property for authentication to occur.
//        /// In your delegate you should construct an authentication principal from the user details,
//        /// then create a new AuthenticationTicket using the principal, attach it to the 
//        /// context.AuthenticationTicket property and finally call context.HandleResponse();
//        /// </remarks>
//        public Func<ValidateCredentialsContext, Task> OnValidateCredentials { get; set; } = context => Task.FromResult(0);

//        public virtual Task AuthenticationFailed(AuthenticationFailedContext context) => OnAuthenticationFailed(context);

//        public virtual Task ValidateCredentials(ValidateCredentialsContext context) => OnValidateCredentials(context);
//    }
//}
