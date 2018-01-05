using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class AnalyticRepository : BaseRepository<Analytic>, IAnalyticRepository
    {
        public AnalyticRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int id)
        {
            return context.Analytics.Any(x => x.Id == id);
        }

        public IList<Allocation> GetResources(int id)
        {
            return context.Allocations.Where(x => x.AnalyticId == id).Include(x => x.Employee).ToList().AsReadOnly();
        }

        public Analytic GetLastAnalytic(int costCenterId)
        {
            return context.Analytics.Where(x => x.CostCenterId == costCenterId).OrderByDescending(x => x.TitleId).Include(x => x.CostCenter).FirstOrDefault();
        }

        public bool ExistTitle(string analyticTitle)
        {
            return context.Analytics.Any(x => x.Title.Equals(analyticTitle));
        }
    }
}
