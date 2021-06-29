using System;
using System.Collections.Generic;
using Sofco.Domain.Enums;

namespace Sofco.Domain.DTO
{
    public class LicenseSearchParams
    {
        public int? EmployeeId { get; set; }

        public int? LicenseTypeId { get; set; }
        
        public DateTime? dateSince { get; set; }

        public DateTime? dateTo { get; set; }
        
    }
}
