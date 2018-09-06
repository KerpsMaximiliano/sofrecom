using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class WorkTimeApprovalApproverService : IWorkTimeApprovalApproverService
    {
        private readonly IUserApproverRepository userApproverRepository;

        private readonly IUserData userData;

        public WorkTimeApprovalApproverService(IUserApproverRepository userApproverRepository, IUserData userData)
        {
            this.userApproverRepository = userApproverRepository;
            this.userData = userData;
        }

        public Response<List<UserSelectListItem>> GetApprovers(UserApproverQuery query)
        {
            if (query.AnalyticId == 0)
            {
                return new Response<List<UserSelectListItem>>{ Data = new List<UserSelectListItem>() };
            }

            var workTimeApprovals = userApproverRepository.GetByAnalyticId(query.AnalyticId, UserApproverType.WorkTime);

            var userIds = workTimeApprovals.Select(s => s.ApproverUserId).Distinct().ToList();

            var users = userIds.Select(userId => userData.GetUserLiteById(userId))
                .Select(userLite => new UserSelectListItem
                {
                    Id = userLite.Id.ToString(),
                    Text = userLite.Name
                })
                .ToList();

            return new Response<List<UserSelectListItem>>
            {
                Data = users
            };
        }
    }
}
