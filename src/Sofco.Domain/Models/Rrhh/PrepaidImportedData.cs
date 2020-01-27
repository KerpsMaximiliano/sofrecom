using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.Rrhh
{
    public class PrepaidImportedData : BaseEntity
    {
        public DateTime Period { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeNumber { get; set; }

        public int PrepaidBeneficiaries { get; set; }

        public int TigerBeneficiaries { get; set; }

        public string PrepaidPlan { get; set; }

        public string TigerPlan { get; set; }

        public decimal PrepaidCost { get; set; }

        public decimal TigerCost { get; set; }

        public decimal CostDifference => PrepaidCost - TigerCost;

        public string Dni { get; set; }

        public string Cuil { get; set; }

        public string Comments { get; set; }

        public PrepaidImportedDataStatus Status { get; set; }

        public DateTime Date { get; set; }

        public int? PrepaidId { get; set; }

        public Prepaid Prepaid { get; set; }

        public bool Closed { get; set; }
    }
}
