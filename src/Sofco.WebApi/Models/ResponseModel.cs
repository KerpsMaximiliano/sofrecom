using System.Collections.Generic;
using Sofco.Common.Domains;

namespace Sofco.WebApi.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }

        public object Data { get; set; }

        public IEnumerable<ResultError> Errors { get; set; }
    }
}
