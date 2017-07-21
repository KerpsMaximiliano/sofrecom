using AutoMapper;
using Sofco.Model.Models;
using Sofco.WebApi.Models;

namespace Sofco.WebApi.Config
{
    public class AutoMapperConfiguration : Profile
    {
        protected override void Configure()
        {
            CreateMap<RoleModel, Role>().ReverseMap();
            CreateMap<UserGroupModel, UserGroup>().ReverseMap();
        }
    }
}
