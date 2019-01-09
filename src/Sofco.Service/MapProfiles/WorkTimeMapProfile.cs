using AutoMapper;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Service.MapProfiles
{
    public class WorkTimeMapProfile : Profile
    {
        public WorkTimeMapProfile()
        {
            CreateMap<WorkTime, WorkTimeControlResourceDetailModel>()
                .ForMember(d => d.TaskDescription, s => s.MapFrom(x => x.Task.Description))
                .ForMember(d => d.Reference, s => s.MapFrom(x => x.Reference))
                .ForMember(d => d.CategoryDescription, s => s.MapFrom(x => x.Task.Category.Description))
                .ForMember(d => d.RegisteredHours, s => s.MapFrom(x => x.Hours))
                .ForMember(d => d.Date, s => s.MapFrom(x => x.Date));

            CreateMap<WorkTime, WorkTimeControlResourceModel>()
                .ForMember(d => d.Id, s => s.MapFrom(x => x.EmployeeId))
                .ForMember(d => d.Analytic, s => s.MapFrom(x => x.Analytic.Title))
                .ForMember(d => d.EmployeeName, s => s.MapFrom(x => x.Employee.Name))
                .ForMember(d => d.EmployeeNumber, s => s.MapFrom(x => x.Employee.EmployeeNumber));
        }
    }
}
