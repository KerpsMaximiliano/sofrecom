using Sofco.Model.Models.Common;
using Sofco.Model.Models.Rrhh;

namespace Sofco.Model.Relationships
{
    public class LicenseFile
    {
        public int LicenseId { get; set; }
        public License License { get; set; }

        public int FileId { get; set; }
        public File File { get; set; }
    }
}
