using AutoMapper;
using Sofco.Core.Models.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.MapProfiles
{
    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            CreateMap<User, UserModel>();

            CreateMap<User, UserDetailModel>();
        }
    }
}
