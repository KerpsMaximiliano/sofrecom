using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }
        public string AccountName { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string Cuit { get; set; }
        public string Service { get; set; }
        public string Project { get; set; }
        public string ProjectId { get; set; }
        public string Analytic { get; set; }

        public IList<InvoiceDetailViewModel> Details { get; set; }

        public Invoice CreateDomain()
        {
            var domain = new Invoice();

            domain.AccountName = AccountName;
            domain.Address = Address;
            domain.Zipcode = Zipcode;
            domain.City = City;
            domain.Province = Province;
            domain.Country = Country;
            domain.Cuit = Cuit;
            domain.Service = Service;
            domain.Project = Project;
            domain.ProjectId = ProjectId;
            domain.Analytic = Analytic;

            foreach (var detail in Details) {
                domain.Details.Add(new InvoiceDetail { Description = detail.Description, Quantity = detail.Quantity });
            }

            return domain;
        }
    }

    public class InvoiceDetailViewModel
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
