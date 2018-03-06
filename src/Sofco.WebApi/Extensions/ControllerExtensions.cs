﻿using System.Linq;
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

        public static IActionResult CreateResponse(this Controller controller, Response response)
        {
            if (response.HasErrors())
                return controller.BadRequest(response);

            return controller.Ok(response);
        }
    }
}