using Sofco.Core.DAL.Rrhh;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Relationships;

namespace Sofco.DAL.Repositories.Rrhh
{
    public class LicenseRepository : BaseRepository<License>, ILicenseRepository
    {
        public LicenseRepository(SofcoContext context) : base(context)
        {
        }

        public void AddFile(LicenseFile licenseFile)
        {
            context.LicenseFiles.Add(licenseFile);
        }
    }
}
