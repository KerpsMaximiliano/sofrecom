﻿using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ResourceBilling : BaseEntity
    {
        public string Profile { get; set; }

        public int? SeniorityId { get; set; }
        public Seniority Seniority { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// 1 = Mes
        /// 2 = Hora
        /// </summary>
        public int MonthHour { get; set; }

        public int Quantity { get; set; }

        public decimal Amount { get; set; }

        public decimal SubTotal { get; set; }

        public string HitoCrmId { get; set; }

        public int ManagementReportBillingId { get; set; }
        public ManagementReportBilling ManagementReportBilling { get; set; }
    }
}
