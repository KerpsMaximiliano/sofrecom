using System.Collections.Generic;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Common;

namespace Sofco.Core.DAL.Common
{
    public interface IUserApproverRepository
    {
        void Save(List<UserApprover> userApprovers);

        void Delete(int userApproverId);

        List<UserApprover> GetByAnalyticId(int analyticId, UserApproverType type);

        List<Analytic> GetByAnalyticApprover(int currentUserId, UserApproverType type);

        IList<User> GetByUserId(int userId, UserApproverType type);
    }
}