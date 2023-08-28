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

        public ICollection<LicenseType> GetAllActivesReadOnly()
        {
            return context.LicenseTypes.Where(x => x.Active && x.ListEmp).ToList().AsReadOnly();
        }

        public List<LicenseType> GetAllListRrhh()
        {            
            var query = context.LicenseTypes
                        .Where(x => x.Active && 
                                    x.ListRRHH.Equals(true) || 
                                    x.ListEmp.Equals(true))
                        .ToList();
            return query;
        }

        public List<LicenseType> GetAllListEmp()
        {
            var query = context.LicenseTypes
                        .Where(x => x.Active && 
                                    x.ListRRHH.Equals(false) && 
                                    x.ListEmp)
                        .ToList();
            return query;
        }
    }
}
