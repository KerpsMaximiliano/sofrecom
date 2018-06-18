using System;
using Sofco.Model.Models.Billing;
using Sofco.Model.Relationships;

namespace Sofco.Core.Models.Billing
{
    public class CertificateFileModel
    {
        public CertificateFileModel(SolfacCertificate solfacCertificate)
        {
            Id = solfacCertificate.CertificateId;
            FileId = solfacCertificate.Certificate.FileId.GetValueOrDefault();
            Name = solfacCertificate.Certificate.Name;

            if (solfacCertificate.Certificate.File != null)
            {
                FileName = solfacCertificate.Certificate.File.FileName;
                CreationDate = solfacCertificate.Certificate.File.CreationDate;
            }
        }

        public CertificateFileModel(Certificate certificate)
        {
            Id = certificate.Id;
            FileId = certificate.FileId.GetValueOrDefault();
            Name = certificate.Name;

            if (certificate.File != null)
            {
                FileName = certificate.File.FileName;
                CreationDate = certificate.File.CreationDate;
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string FileName { get; set; }

        public DateTime CreationDate { get; set; }

        public int FileId { get; set; }
    }
}
