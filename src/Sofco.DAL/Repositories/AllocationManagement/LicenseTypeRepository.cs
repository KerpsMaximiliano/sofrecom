using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class LicenseTypeRepository : BaseRepository<LicenseType>, ILicenseTypeRepository
    {
        public LicenseTypeRepository(SofcoContext context) : base(context)
        {
        }
    }
}
