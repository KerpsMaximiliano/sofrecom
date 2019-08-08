using System.Collections.Generic;
using Sofco.Core.Models.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.Common
{
    public interface IDelegationRepository : IBaseRepository<Delegation>
    {
        bool Exist(DelegationAddModel model, int userId);
        IList<Delegation> GetByUserId(int userId);
        IList<Delegation> GetByGrantedUserIdAndType(int currentUserId, DelegationType type);
    }
}
