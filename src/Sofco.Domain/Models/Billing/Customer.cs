using System.Collections.Generic;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Domain.Models.Billing
{
    public class Customer
    {
        public int Id { get; set; }

        public string CrmId { get; set; }

        public string Name { get; set; }

        public string Telephone { get; set; }

        public string Address { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string Country { get; set; }

        public string Cuit { get; set; }

        public string CurrencyId { get; set; }

        public string CurrencyDescription { get; set; }

        public string Contact { get; set; }

        public int? PaymentTermCode { get; set; }

        public string PaymentTermDescription { get; set; }

        public string OwnerId { get; set; }

        public bool Active { get; set; }

        public IList<JobSearch> JobSearchs { get; set; }
        public IList<Applicant> Applicants { get; set; }
    }
}
