namespace Sofco.WebApi.Models.Billing
{
    public class CustomerCrm
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string CUIT { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyDescription { get; set; }
    }
}
