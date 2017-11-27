using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.WebApi.Extensions
{
    public static class ControllerExtensions
    {
        public static Response GetErrors(this Controller controller)
        {
            var response = new Response();

            if (!controller.ModelState.IsValid)
            {
                var errors = controller.ModelState.Select(x => x.Value.Errors).ToList();

                foreach (var error in errors)
                {
                    foreach (var item in error)
                    {
                        response.Messages.Add(new Message(item.ErrorMessage, MessageType.Error));
                    }
                }
            }

            return response;
        }

        public static string GetUserMail(this Controller controller)
        {
            var username = controller.User.Identity.Name.Split('@');

            var mail = $"{username[0]}@sofrecom.com.ar";

            return mail;
        }

        public static string GetUserName(this Controller controller)
        {
            var username = controller.User.Identity.Name.Split('@');

            return username[0];
        }
    }
}