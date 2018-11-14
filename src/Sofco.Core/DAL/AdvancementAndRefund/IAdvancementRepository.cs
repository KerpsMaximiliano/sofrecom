using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.DAL.AdvancementAndRefund
{
    public interface IAdvancementRepository : IBaseRepository<Advancement>
    {
        bool Exist(int id);
        Advancement GetById(int id);
    }
}
