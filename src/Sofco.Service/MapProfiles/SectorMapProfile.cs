using AutoMapper;
using Sofco.Core.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.MapProfiles
{
    public class SectorMapProfile : Profile
    {
        public SectorMapProfile()
        {
            CreateMap<Sector, SectorModel>();
        }
    }
}
