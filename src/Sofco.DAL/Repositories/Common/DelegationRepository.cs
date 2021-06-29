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

        public bool Exist(DelegationAddModel model, int userId, int? userSourceId)
        {
            if (model.AnalyticSourceId.HasValue && model.AnalyticSourceId.Value > 0)
            {
                if (userSourceId.HasValue && userSourceId.Value > 0)
                {
                    return context.Delegations.Any(x => x.UserId == userId &&
                                                          x.GrantedUserId == model.GrantedUserId &&
                                                          x.Type == model.Type &&
                                                          x.AnalyticSourceId.Value == model.AnalyticSourceId.Value &&
                                                          x.UserSourceId.Value == userSourceId.Value &&
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
                if (userSourceId.HasValue && userSourceId.Value > 0)
                {
                    return context.Delegations.Any(x => x.UserId == userId && x.GrantedUserId == model.GrantedUserId && x.Type == model.Type && x.UserSourceId == userSourceId);
                }
                else
                {
                    return context.Delegations.Any(x => x.UserId == userId && x.GrantedUserId == model.GrantedUserId && x.Type == model.Type);
                }
            }
        }

        public IList<Delegation> GetByUserId(int userId)
        {
            return context.Delegations
                .Include(x => x.User)
                .Include(x => x.GrantedUser)
                .Where(x => x.UserId == userId).ToList();
        }

        public IList<Delegation> GetByUserId(int userId, DelegationType type)
        {
            return context.Delegations
                .Include(x => x.User)
                .Include(x => x.GrantedUser)
                .Where(x => x.UserId == userId && x.Type == type)
                .ToList();
        }

        public IList<Delegation> GetByGrantedUserIdAndType(int userId, DelegationType type)
        {
            return context.Delegations.Include(x => x.User).Include(x => x.GrantedUser).Where(x => x.GrantedUserId == userId && x.Type == type).ToList();
        }

        public IList<Delegation> GetByGrantedUserIdAndTypeDelegation(int userId)
        {
            var response = context.Delegations.Include(x => x.User).Include(x => x.GrantedUser).Where(x => x.GrantedUserId == userId).ToList();

            return response;
        }

        public bool ExistByGrantedUserIdAndType(int userId, DelegationType type)
        {
            return context.Delegations.Include(x => x.User).Include(x => x.GrantedUser).Any(x => x.GrantedUserId == userId && x.Type == type);
        }

        public IList<Delegation> GetByEmployeeSourceId(int employeeId, DelegationType type)
        {
            return context.Delegations.Include(x => x.GrantedUser).Where(x => x.EmployeeSourceId == employeeId && x.Type == type).ToList();
        }

        public IList<Delegation> GetByEmployeeSourceId(List<int> employeeIds, DelegationType type)
        {
            return context.Delegations.Include(x => x.GrantedUser).Where(x => employeeIds.Contains(x.EmployeeSourceId.GetValueOrDefault()) && x.Type == type).ToList();
        }

        public IList<Delegation> GetByEmployeeSourceId(int employeeId, int analyticId, DelegationType type)
        {
            return context.Delegations.Include(x => x.GrantedUser)
                .Where(x => x.AnalyticSourceId == analyticId && x.EmployeeSourceId == employeeId && x.Type == type)
                .ToList();
        }

        public IList<Delegation> GetByGrantedUserIdAndType(int grantedUserId, List<DelegationType> types)
        {
            return context.Delegations
                .Include(x => x.User)
                .Include(x => x.GrantedUser)
                .Where(x => x.GrantedUserId == grantedUserId && types.Contains(x.Type))
                .ToList();
        }
    }
}
