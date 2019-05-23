﻿using System;
using Sofco.Domain.Enums;

namespace Sofco.Domain.Models.Rrhh
{
    public class PrepaidImportedData : BaseEntity
    {
        public string Prepaid { get; set; }

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

        public string Dni { get; set; }

        public string Cuil { get; set; }

        public string Comments { get; set; }

        public PrepaidImportedDataStatus Status { get; set; }

        public DateTime Date { get; set; }
    }
}
