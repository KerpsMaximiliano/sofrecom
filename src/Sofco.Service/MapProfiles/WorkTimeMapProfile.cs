using AutoMapper;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Service.MapProfiles
{
    public class WorkTimeMapProfile : Profile
    {
        public WorkTimeMapProfile()
        {
            CreateMap<WorkTimeApprovalModel, WorkTimeApproval>();

            CreateMap<WorkTimeApproval, WorkTimeApprovalModel>();

            CreateMap<WorkTimeApprovalEmployee, WorkTimeApprovalEmployeeModel>();
        }
    }
}
