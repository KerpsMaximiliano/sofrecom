using Sofco.Model.Models.Billing;

namespace Sofco.Model.Relationships
{
    public class SolfacCertificate
    {
        public int SolfacId { get; set; }

        public Solfac Solfac { get; set; }

        public int CertificateId { get; set; }

        public Certificate Certificate { get; set; }
    }
}
