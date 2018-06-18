using System.Collections.Generic;
using Sofco.Common.Domains;

namespace Sofco.Core.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }

        public object Data { get; set; }

        public IEnumerable<ResultError> Errors { get; set; }
    }
}
