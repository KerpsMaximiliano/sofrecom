using AutoMapper;
using Newtonsoft.Json;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.MapProfiles
{
    public class EmployeeSyncActionMapProfile : Profile
    {
        public EmployeeSyncActionMapProfile()
        {
            CreateMap<EmployeeSyncAction, EmployeeNewsModel>()
                .ForMember(d => d.Name, s => s.ResolveUsing(x =>
                {
                    var data = JsonConvert.DeserializeObject<Employee>(x.EmployeeData);

                    return data.Name;
                }));
        }
    }
}
