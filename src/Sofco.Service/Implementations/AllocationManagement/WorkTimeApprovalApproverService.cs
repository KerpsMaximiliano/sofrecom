using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class WorkTimeApprovalApproverService : IWorkTimeApprovalApproverService
    {
        private readonly IWorkTimeApprovalRepository workTimeApprovalRepository;

        private readonly IUserData userData;

        public WorkTimeApprovalApproverService(IWorkTimeApprovalRepository workTimeApprovalRepository, IUserData userData)
        {
            this.workTimeApprovalRepository = workTimeApprovalRepository;
            this.userData = userData;
        }

        public Response<List<UserSelectListItem>> GetApprovers(WorkTimeApprovalQuery query)
        {
            if (query.AnalyticId == 0)
            {
                return new Response<List<UserSelectListItem>>{ Data = new List<UserSelectListItem>() };
            }

            var workTimeApprovals = workTimeApprovalRepository.GetByAnalyticId(query.AnalyticId);

            var userIds = workTimeApprovals.Select(s => s.ApprovalUserId).Distinct().ToList();

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
