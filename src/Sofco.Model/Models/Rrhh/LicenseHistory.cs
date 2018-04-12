using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Models.Rrhh
{
    public class LicenseHistory : History
    {
        public LicenseStatus LicenseStatusFrom { get; set; }

        public LicenseStatus LicenseStatusTo { get; set; }

        public int LicenseId { get; set; }

        public License License { get; set; }
    }
}
