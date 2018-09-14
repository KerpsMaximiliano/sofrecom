using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Core.Managers.UserApprovers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class UserApproverEmployeeService : IUserApproverEmployeeService
    {
        private readonly IWorkTimeApproverEmployeeManager workTimeApproverEmployeeManager;

        private readonly ILicenseApproverEmployeeManager licenseApproverEmployeeManager;

        private readonly IMapper mapper;

        private Dictionary<UserApproverType, Func<UserApproverQuery, List<UserApproverEmployee>>> approverEmployeeManager;

        public UserApproverEmployeeService(IMapper mapper, IWorkTimeApproverEmployeeManager workTimeApproverEmployeeManager, ILicenseApproverEmployeeManager licenseApproverEmployeeManager)
        {
            this.workTimeApproverEmployeeManager = workTimeApproverEmployeeManager;
            this.licenseApproverEmployeeManager = licenseApproverEmployeeManager;
            this.mapper = mapper;
            SetApproverMangerDicts();
        }

        public Response<List<UserApproverEmployeeModel>> Get(UserApproverQuery query, UserApproverType type)
        {
            var employees = approverEmployeeManager.ContainsKey(type) ? approverEmployeeManager[type](query) : null;

            return new Response<List<UserApproverEmployeeModel>>{ Data = Translate(employees) };
        }

        private void SetApproverMangerDicts()
        {
            approverEmployeeManager = 
                new Dictionary<UserApproverType, Func<UserApproverQuery, List<UserApproverEmployee>>>
                {
                    {UserApproverType.WorkTime, workTimeApproverEmployeeManager.Get},
                    {UserApproverType.LicenseAuthorizer, licenseApproverEmployeeManager.Get}
                };
        }

        private List<UserApproverEmployeeModel> Translate(List<UserApproverEmployee> data)
        {
            return mapper.Map<List<UserApproverEmployee>, List<UserApproverEmployeeModel>>(data);
        }
    }
}
