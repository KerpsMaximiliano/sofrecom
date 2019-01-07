using Sofco.Domain.Enums;
using System;

namespace Sofco.Domain.DTO
{
    public class SolfacParams
    {
        public string CustomerId { get; set; }
        public string ServiceId { get; set; }
        public string ProjectId { get; set; }
        public string Analytic { get; set; }
        public SolfacStatus Status { get; set; }
        public string ManagerId { get; set; }
        public DateTime? DateSince { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
