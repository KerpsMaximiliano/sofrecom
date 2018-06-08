using System;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class CertificateModel
    {
        public string Name { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int Year { get; set; }

        public Certificate CreateDomain(string userName)
        {
            var domain = new Certificate();

            domain.Name = Name;
            domain.ClientExternalId = ClientExternalId;
            domain.ClientExternalName = ClientExternalName;
            domain.Year = Year;

            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;

            return domain;
        }
    }
}
