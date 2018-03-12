﻿using System;
using Sofco.Model.Models.Rrhh;

namespace Sofco.Core.Models.Rrhh
{
    public class LicenseListItem
    {
        public LicenseListItem(License license)
        {
            Id = license.Id;
            StartDate = license.StartDate;
            EndDate = license.EndDate;
            WithPayment = license.WithPayment;
            Status = license.Status.ToString();
            HasCertificate = license.HasCertificate;

            Days = EndDate.Date.Subtract(StartDate.Date).Days + 1;

            if (license.Employee != null)
            {
                EmployeeId = license.EmployeeId;
                EmployeeName = license.Employee.Name;
            }

            if (license.Manager != null)
            {
                ManagerId = license.ManagerId;
                ManagerName = license.Manager.Name;
            }

            if (license.Type != null)
            {
                LicenseTypeName = license.Type.Description;
            }
        }

        public bool HasCertificate { get; set; }

        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public int ManagerId { get; set; }

        public string ManagerName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string LicenseTypeName { get; set; }

        public int Days { get; set; }

        public bool WithPayment { get; set; }

        public string Status { get; set; }
    }
}
