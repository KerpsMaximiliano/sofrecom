using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Framework.ValidationHelpers.Billing
{
    public static class InvoiceValidationHelper
    {
        public static void ValidateCuit(Response<Invoice> response, Invoice invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.Cuit))
            {
                response.AddError(Resources.Billing.Invoice.CuitRequired);
            }
        }

        public static void ValidateAddress(Response<Invoice> response, Invoice invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.Address))
            {
                response.AddError(Resources.Billing.Invoice.AddressRequired);
            }
        }

        public static void ValidateZipCode(Response<Invoice> response, Invoice invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.Zipcode))
            {
                response.AddError(Resources.Billing.Invoice.ZipCodeRequired);
            }
        }

        public static void ValidateCity(Response<Invoice> response, Invoice invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.City))
            {
                response.AddError(Resources.Billing.Invoice.CityRequired);
            }
        }

        public static void ValidateProvince(Response<Invoice> response, Invoice invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.Province))
            {
                response.AddError(Resources.Billing.Invoice.ProvinceRequired);
            }
        }

        public static void ValidateCountry(Response<Invoice> response, Invoice invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.Country))
            {
                response.AddError(Resources.Billing.Invoice.CountryRequired);
            }
        }
    }
}
