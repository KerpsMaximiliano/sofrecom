using AutoMapper;
using Sofco.Core.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Service.MapProfiles
{
    public class AreaMapProfile : Profile
    {
        public AreaMapProfile()
        {
            CreateMap<Area, AreaModel>();
        }
    }
}
