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

        IList<User> GetApproverByUserId(int userId, UserApproverType type);

        IList<User> GetApproverByEmployeeId(int employeeId, UserApproverType type);

        IList<User> GetApproverByEmployeeIdAndAnalyticId(int employeeId, int analyticId, UserApproverType type);

        List<UserApprover> GetByApproverUserId(int approverUserId, UserApproverType type);

        List<UserApprover> GetByEmployeeIds(List<int> employeeIds, UserApproverType type);

        bool HasUserAuthorizer(int userId, UserApproverType type);

        User GetAuthorizerForLicenses(string managerUsername, int employeeId);
    }
}