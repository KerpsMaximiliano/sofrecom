using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Common.Security;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeEndNotificationService : IEmployeeEndNotificationService
    {
        private readonly IEmployeeEndNotificationRepository repository;

        private readonly IEmployeeData employeeData;

        private readonly IUserData userData;

        private readonly IMapper mapper;

        public EmployeeEndNotificationService(IEmployeeEndNotificationRepository repository, IMapper mapper, IEmployeeData employeeData, IUserData userData)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.employeeData = employeeData;
            this.userData = userData;
        }

        public Response<List<EmployeeEndNotificationModel>> Get(EmployeeEndNotificationParameters parameters)
        {
            if (parameters == null)
            {
                parameters = new EmployeeEndNotificationParameters();
            } 

            ResolverApplicantUser(parameters);

            var result = repository.GetByParameters(parameters);

            return new Response<List<EmployeeEndNotificationModel>>
            {
                Data = Translate(result)
            };
        }

        private List<EmployeeEndNotificationModel> Translate(List<EmployeeEndNotification> data)
        {
            var result = mapper.Map<List<EmployeeEndNotification>, List<EmployeeEndNotificationModel>>(data);

            result.ForEach(x =>
            {
                var employee = employeeData.GetByEmployeeId(x.EmployeeId);

                x.EmployeeName = employee.Name;
                x.EmployeeNumber = employee.EmployeeNumber;
                x.ApplicantName = userData.GetById(x.ApplicantUserId).Name;
            });

            return result;
        }

        private void ResolverApplicantUser(EmployeeEndNotificationParameters parameters)
        {
            if(!parameters.ApplicantEmployeeId.HasValue) return;

            var applicantEmployee = employeeData.GetByEmployeeId(parameters.ApplicantEmployeeId.Value);

            if(applicantEmployee == null) return;

            var userName = applicantEmployee.Email.Split('@').First();

            parameters.ApplicantUserId = userData.GetUserLiteByUserName(userName)?.Id;
        }
    }
}
