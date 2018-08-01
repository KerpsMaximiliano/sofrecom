using AutoMapper;
using Newtonsoft.Json;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Service.MapProfiles
{
    public class EmployeeSyncActionMapProfile : Profile
    {
        public EmployeeSyncActionMapProfile()
        {
            CreateMap<Employee, EmployeeSyncAction>()
                .ForMember(d => d.EmployeeData, s => s.MapFrom(x => JsonConvert.SerializeObject(x)));

            CreateMap<EmployeeSyncAction, EmployeeNewsModel>()
                .ForMember(d => d.Name, s => s.ResolveUsing(x =>
                {
                    var data = JsonConvert.DeserializeObject<Employee>(x.EmployeeData);

                    return data.Name;
                }));
        }
    }
}
