using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.DAL.Repositories.Common
{
    public class DelegationRepository : BaseRepository<Delegation>, IDelegationRepository
    {
        public DelegationRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(DelegationAddModel model, int userId)
        {
            if (model.AnalyticSourceId.HasValue && model.AnalyticSourceId.Value > 0)
            {
                if (model.UserSourceId.HasValue && model.UserSourceId.Value > 0)
                {
                    return context.Delegations.Any(x => x.UserId == userId &&
                                                          x.GrantedUserId == model.GrantedUserId &&
                                                          x.Type == model.Type &&
                                                          x.AnalyticSourceId.Value == model.AnalyticSourceId.Value &&
                                                          x.UserSourceId.Value == model.UserSourceId.Value &&
                                                          x.SourceType == model.SourceType);
                }
                else
                {
                    return context.Delegations.Any(x => x.UserId == userId &&
                                                          x.GrantedUserId == model.GrantedUserId &&
                                                          x.Type == model.Type &&
                                                          x.AnalyticSourceId.Value == model.AnalyticSourceId.Value &&
                                                          x.SourceType == model.SourceType);
                }
            }
            else
            {
                return context.Delegations.Any(x => x.UserId == userId && x.GrantedUserId == model.GrantedUserId && x.Type == model.Type && x.UserSourceId == model.UserSourceId);
            }
        }

        public IList<Delegation> GetByUserId(int userId)
        {
            return context.Delegations
                .Include(x => x.User)
                .Include(x => x.GrantedUser)
                .Where(x => x.UserId == userId).ToList();
        }

        public IList<Delegation> GetByGrantedUserIdAndType(int currentUserId, DelegationType type)
        {
            return context.Delegations.Where(x => x.GrantedUserId == currentUserId && x.Type == type).ToList();
        }
    }
}
