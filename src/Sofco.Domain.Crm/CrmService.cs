﻿using System;

namespace Sofco.Domain.Crm
{
    public class CrmService
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string Industry { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Manager { get; set; }

        public string ManagerId { get; set; }

        public string ServiceType { get; set; }

        public int ServiceTypeId { get; set; }

        public string SolutionType { get; set; }

        public int SolutionTypeId { get; set; }

        public string TechnologyType { get; set; }

        public int TechnologyTypeId { get; set; }

        public string Analytic { get; set; }

        public int StateCode { get; set; }

        public string Description { get; set; }
    }
}
