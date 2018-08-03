using System;
using Sofco.Domain.DTO;

namespace Sofco.Core.Models.Billing
{
    public class UpdateSolfacCash
    {
        public int UserId { get; set; }

        public DateTime? CashedDate { get; set; }

        public SolfacStatusParams CreateStatusParams()
        {
            var parameters = new SolfacStatusParams();

            parameters.UserId = UserId;
            parameters.CashedDate = CashedDate;

            return parameters;
        }
    }
}
