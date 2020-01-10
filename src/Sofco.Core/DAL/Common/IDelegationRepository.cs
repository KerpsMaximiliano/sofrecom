using System.Collections.Generic;
using Sofco.Core.Models.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.Common
{
    public interface IDelegationRepository : IBaseRepository<Delegation>
    {
        bool Exist(DelegationAddModel model, int userId, int? userSourceId);
        IList<Delegation> GetByUserId(int userId);
        IList<Delegation> GetByUserId(int userId, DelegationType type);
        IList<Delegation> GetByGrantedUserIdAndType(int currentUserId, DelegationType type);
        bool ExistByGrantedUserIdAndType(int userId, DelegationType type);
        IList<Delegation> GetByEmployeeSourceId(int employeeId, DelegationType licenseAuthorizer);
        IList<Delegation> GetByEmployeeSourceId(List<int> employeeIds, DelegationType type);
        IList<Delegation> GetByEmployeeSourceId(int employeeId, int analyticId, DelegationType type);
        IList<Delegation> GetByGrantedUserIdAndType(int grantedUserId, List<DelegationType> types);
    }
}
