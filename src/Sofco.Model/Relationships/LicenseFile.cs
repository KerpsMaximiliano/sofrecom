using Sofco.Domain.Models.Common;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Domain.Relationships
{
    public class LicenseFile
    {
        public int LicenseId { get; set; }
        public License License { get; set; }

        public int FileId { get; set; }
        public File File { get; set; }
    }
}
