using System;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class CertificateEditViewModel
    {
        public CertificateEditViewModel()
        {
            
        }

        public CertificateEditViewModel(Certificate domain)
        {
            Id = domain.Id;
            Name = domain.Name;
            ClientExternalId = domain.ClientExternalId;
            ClientExternalName = domain.ClientExternalName;
            Year = domain.Year;

            if (domain.File != null)
            {
                FileId = domain.FileId.GetValueOrDefault();
                FileName = domain.File.FileName;
                CreationDate = domain.File.CreationDate.ToString("d");
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int Year { get; set; }

        public int FileId { get; set; }

        public string FileName { get; set; }

        public string CreationDate { get; set; }

        public Certificate CreateDomain(string userName)
        {
            var domain = new Certificate();

            domain.Id = Id;
            domain.Name = Name;
            domain.ClientExternalId = ClientExternalId;
            domain.ClientExternalName = ClientExternalName;
            domain.Year = Year;

            if (FileId > 0) domain.FileId = FileId;

            domain.UpdateDate = DateTime.UtcNow;
            domain.UpdateByUser = userName;

            return domain;
        }
    }
}
