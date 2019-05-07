using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.DAL.Repositories.ManagementReport
{
   public class ContractedDetailRepository : BaseRepository<ContratedDetail>, IContractedDetailRepository
    {
        public ContractedDetailRepository(SofcoContext context) : base(context)
        {

        }

        public List<ContratedDetail> GetByAnalytic(int IdAnalytic)
        {
            var Contracted = context.ContractedDetails
                                        .Where(x => x.IdAnalytic == IdAnalytic)
                                        .ToList();
            return Contracted;
        }
    }
}
