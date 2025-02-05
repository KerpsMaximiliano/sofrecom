﻿using System;

namespace Sofco.Domain.Crm
{
    public class CrmAccount
    {
        public Guid Id { get; set; }

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

        public int PaymentTermCode { get; set; }

        public string PaymentTermDescription { get; set; }

        public string StatusCode { get; set; }

        public decimal RelCommercial { get; set; }

        public Guid OwnerId { get; set; }
    }
}
