using System;
using System.Collections.Generic;

namespace Sofco.Domain.DTO
{
    public class HitoParameters
    {
        public string ExternalHitoId { get; set; }
        public string Name { get; set; }
        public decimal? Ammount { get; set; }
        public decimal AmmountFirstHito { get; set; }
        public string StatusCode { get; set; }
        public DateTime? StartDate { get; set; }
        public int? Month { get; set; }
        public string ProjectId { get; set; }
        public string OpportunityId { get; set; }
        public string ManagerId { get; set; }
        public string MoneyId { get; set; }
    }

    public class HitoAmmountParameter
    {
        public HitoAmmountParameter()
        {
        }

        public HitoAmmountParameter(string id, string projectId, decimal amount, string name, int month,
            DateTime? date)
        {
            Id = id;
            ProjectId = projectId;
            Ammount = amount;
            Name = name;
            Month = month;
            Date = date.GetValueOrDefault();
        }

        public string Id { get; set; }

        public string ProjectId { get; set; }

        public decimal? Ammount { get; set; }

        public string Name { get; set; }

        public int Month { get; set; }

        public DateTime Date { get; set; }
    }
}
