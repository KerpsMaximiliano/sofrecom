using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Models.Admin;
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
            if (query.CustomerId == Guid.Empty)
            {
                return new Response<List<UserSelectListItem>>{ Data = new List<UserSelectListItem>() };
            }

            var analytics = unitOfWork.AnalyticRepository.GetByClient(query.CustomerId.ToString());

            var serviceIds = analytics.Select(s => Guid.Parse(s.ServiceId)).ToList();

            if (query.ServiceId != Guid.Empty)
            {
                serviceIds = serviceIds.Where(s => s == query.ServiceId).ToList();
            }

            var workTimeApprovals = workTimeApprovalRepository.GetByServiceIds(serviceIds);

            var userIds = workTimeApprovals.Select(s => s.ApprovalUserId).Distinct().ToList();

            var users = new List<UserSelectListItem>();

            foreach (var userId in userIds)
            {
                var userLite = userData.GetUserLiteById(userId);

                users.Add(new UserSelectListItem
                {
                    Id = userLite.Id.ToString(),
                    Text = userLite.Name
                });
            }

            return new Response<List<UserSelectListItem>>
            {
                Data = users
            };
        }
    }
}
