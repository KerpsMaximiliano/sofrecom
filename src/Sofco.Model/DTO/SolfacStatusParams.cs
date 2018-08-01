using System;
using Sofco.Domain.Enums;

namespace Sofco.Domain.DTO
{
    public class SolfacStatusParams
    {
        public SolfacStatus Status { get; set; }
        public string Comment { get; set; }
        public string InvoiceCode { get; set; }
        public int UserId { get; set; }
        public DateTime? InvoiceDate;
        public DateTime? CashedDate;

        public SolfacStatusParams()
        {
            
        }

        public SolfacStatusParams(int userApplicantId, SolfacStatus pendingByManagementControl)
        {
            this.UserId = userApplicantId;
            this.Status = pendingByManagementControl;
        }
    }
}
