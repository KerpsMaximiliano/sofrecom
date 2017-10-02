using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sofco.Common.Domains
{
    public class ResultError : ValidationResult
    {
        public string ErrorCode { get; set; }

        public ResultError(string errorMessage)
            : base(errorMessage)
        {
        }

        public ResultError(string errorMessage, IEnumerable<string> memberNames)
            : base(errorMessage, memberNames)
        {
        }

        public ResultError(string errorCode, string errorMessage, IEnumerable<string> memberNames)
            : base(errorMessage, memberNames)
        {
            ErrorCode = errorCode;
        }

        public ResultError(ValidationResult validationResult)
            : base(validationResult)
        {
        }
    }
}