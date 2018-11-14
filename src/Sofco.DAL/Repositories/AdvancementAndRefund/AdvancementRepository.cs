using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.DAL.Repositories.AdvancementAndRefund
{
    public class AdvancementRepository : BaseRepository<Advancement>, IAdvancementRepository
    {
        public AdvancementRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int id)
        {
            return context.Advancements.Any(x => x.Id == id);
        }

        public Advancement GetById(int id)
        {
            return context.Advancements.Include(x => x.Details).SingleOrDefault(x => x.Id == id);
        }
    }
}
