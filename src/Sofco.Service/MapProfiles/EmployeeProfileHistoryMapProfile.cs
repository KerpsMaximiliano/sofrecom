using AutoMapper;
using Newtonsoft.Json;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Service.MapProfiles
{
    public class EmployeeProfileHistoryMapProfile : Profile
    {
        public EmployeeProfileHistoryMapProfile()
        {
            CreateMap<EmployeeProfileHistory, EmployeeProfileHistoryModel>()
                .ForMember(d => d.Fields, s => s.MapFrom(x => x.ModifiedFields))
                .ForMember(d => d.DateTime, s => s.MapFrom(x => x.Created.Value.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'")))
                .ForMember(d => d.EmployeePrevious, s => s.ResolveUsing(x => JsonConvert.DeserializeObject<Employee>(x.EmployeePreviousData)))
                .ForMember(d => d.Employee, s => s.ResolveUsing(x => JsonConvert.DeserializeObject<Employee>(x.EmployeeData)));
        }
    }
}
