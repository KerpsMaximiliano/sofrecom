using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Framework.Managers.UserApprovers
{
    public class RefundApproverEmployeeManager : IRefundApproverEmployeeManager
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly UserApproverType Type = UserApproverType.Refund;

        public RefundApproverEmployeeManager(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public List<ApproverUserDelegate> Get(int userId)
        {
            var list = unitOfWork.UserApproverRepository.GetByUserId(Type, userId);

            var response = new List<ApproverUserDelegate>();

            foreach (var userApprover in list)
            {
                response.Add(new ApproverUserDelegate
                {
                    Id = userApprover.Id,
                    AnalyticId = userApprover.AnalyticId,
                    AnalyticName = $"{userApprover.Analytic?.Title} - {userApprover.Analytic?.Name}",
                    ApproverId = userApprover.UserId,
                    ApproverName = userApprover.ApproverUser?.Name
                });
            }

            return response;
        }
    }
}
