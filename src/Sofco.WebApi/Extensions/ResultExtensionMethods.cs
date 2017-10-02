using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Sofco.Common.Domains;
using Sofco.WebApi.Models;

namespace Sofco.WebApi.Extensions
{
    public static class ResultExtensionMethods
    {
        public static DomainActionResult CreateResponse(this Result result, Controller controller)
        {
            return new DomainActionResult(controller.HttpContext, result);
        }

        public static List<ValidationResult> Validate<T>(this T model)
        {
            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            return validationResults;
        }
    }
}
