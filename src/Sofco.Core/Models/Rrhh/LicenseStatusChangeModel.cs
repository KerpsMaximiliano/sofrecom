using Sofco.Domain.Enums;

namespace Sofco.Core.Models.Rrhh
{
    public class LicenseStatusChangeModel
    {
        public int UserId { get; set; }

        public string Comment { get; set; }

        public LicenseStatus Status { get; set; }

        public bool IsRrhh { get; set; }
    }
}
