using AutoMapper;
using Sofco.Core.Models.Billing;
using Sofco.Model.Models.Billing;

namespace Sofco.Service.MapProfiles
{
    public class SolfacMapProfile : Profile
    {
        public SolfacMapProfile()
        {
            CreateMap<SolfacDelegate, SolfacDelegateModel>();
        }
    }
}
