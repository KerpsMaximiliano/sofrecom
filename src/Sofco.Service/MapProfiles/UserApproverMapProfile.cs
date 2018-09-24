using AutoMapper;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;

namespace Sofco.Service.MapProfiles
{
    public class UserApproverMapProfile : Profile
    {
        public UserApproverMapProfile()
        {
            CreateMap<UserApprover, UserApproverModel>();

            CreateMap<UserApproverModel, UserApprover>()
                .ForMember(d => d.Type, s => s.ResolveUsing(x => UserApproverType.WorkTime));

            CreateMap<UserApproverEmployee, UserApproverEmployeeModel>();
        }
    }
}
