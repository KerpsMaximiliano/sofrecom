using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;

namespace Sofco.Domain.Models.Rrhh
{
    public class LicenseHistory : History
    {
        public LicenseStatus LicenseStatusFrom { get; set; }

        public LicenseStatus LicenseStatusTo { get; set; }

        public int LicenseId { get; set; }

        public License License { get; set; }
    }
}
