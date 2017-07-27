using Microsoft.AspNetCore.Mvc;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using System.Linq;

namespace Sofco.WebApi.Config
{
    public static class ControllerExtensions
    {
        public static Response GetErrors(this Controller controller)
        {
            var respose = new Response();

            if (!controller.ModelState.IsValid)
            {
                var errors = controller.ModelState.Select(x => x.Value.Errors).ToList();

                foreach (var error in errors)
                {
                    foreach (var item in error)
                    {
                        respose.Messages.Add(new Message(item.ErrorMessage, MessageType.Error));
                    }
                }
            }

            return respose;
        }
    }
}



