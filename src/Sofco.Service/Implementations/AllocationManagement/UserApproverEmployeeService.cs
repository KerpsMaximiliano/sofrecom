using AutoMapper;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class UserApproverEmployeeService : IUserApproverEmployeeService
    {
        private readonly IWorkTimeApproverEmployeeManager workTimeApproverEmployeeManager;

        private readonly ILicenseApproverEmployeeManager licenseApproverEmployeeManager;

        private readonly IRefundApproverEmployeeManager refundApproverEmployeeManager;

        private readonly IMapper mapper;

        private Dictionary<UserApproverType, Func<UserApproverQuery, List<UserApproverEmployee>>> approverEmployeeManager;

        private Dictionary<UserApproverType, Func<int, List<ApproverUserDelegate>>> approverManager;

        public UserApproverEmployeeService(IMapper mapper, IWorkTimeApproverEmployeeManager workTimeApproverEmployeeManager,
                                            ILicenseApproverEmployeeManager licenseApproverEmployeeManager, IRefundApproverEmployeeManager refundApproverEmployeeManager)
        {
            this.workTimeApproverEmployeeManager = workTimeApproverEmployeeManager;
            this.licenseApproverEmployeeManager = licenseApproverEmployeeManager;
            this.licenseApproverEmployeeManager = licenseApproverEmployeeManager;
            this.refundApproverEmployeeManager = refundApproverEmployeeManager;
            this.mapper = mapper;
            SetApproverMangerDicts();
        }

        public Response<List<UserApproverEmployeeModel>> Get(UserApproverQuery query, UserApproverType type)
        {
            var employees = approverEmployeeManager.ContainsKey(type) ? approverEmployeeManager[type](query) : null;

            return new Response<List<UserApproverEmployeeModel>> { Data = Translate(employees) };
        }

        public Response<List<ApproverUserDelegate>> GetByUserId(int userId, UserApproverType type)
        {
            var response = new Response<List<ApproverUserDelegate>>();

            response.Data = approverManager[type](userId);

            return response;
        }

        private void SetApproverMangerDicts()
        {
            approverEmployeeManager =
                new Dictionary<UserApproverType, Func<UserApproverQuery, List<UserApproverEmployee>>>
                {
                    { UserApproverType.WorkTime, workTimeApproverEmployeeManager.Get },
                    { UserApproverType.LicenseAuthorizer, licenseApproverEmployeeManager.Get }
                };

            approverManager =
                new Dictionary<UserApproverType, Func<int, List<ApproverUserDelegate>>>
                {
                    { UserApproverType.Refund, refundApproverEmployeeManager.Get },
                };
        }

        private List<UserApproverEmployeeModel> Translate(List<UserApproverEmployee> data)
        {
            return mapper.Map<List<UserApproverEmployee>, List<UserApproverEmployeeModel>>(data);
        }
    }
}
