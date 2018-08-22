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

            CreateMap<WorkTime, WorkTimeControlResourceDetailModel>()
                .ForMember(d => d.TaskDescription, s => s.MapFrom(x => x.Task.Description))
                .ForMember(d => d.CategoryDescription, s => s.MapFrom(x => x.Task.Category.Description))
                .ForMember(d => d.RegisteredHours, s => s.MapFrom(x => x.Hours));
        }
    }
}
