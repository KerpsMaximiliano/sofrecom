using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class ApproversWorkTimeApprovalService : IApproversWorkTimeApprovalService
    {
        private readonly IWorkTimeApprovalRepository workTimeApprovalRepository;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        public ApproversWorkTimeApprovalService(IWorkTimeApprovalRepository workTimeApprovalRepository, IUnitOfWork unitOfWork, IUserData userData)
        {
            this.workTimeApprovalRepository = workTimeApprovalRepository;
            this.unitOfWork = unitOfWork;
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
