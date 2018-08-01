using AutoMapper;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Service.MapProfiles
{
    public class AnalyticMapProfile : Profile
    {
        public AnalyticMapProfile()
        {
            CreateMap<Analytic, AnalyticSearchViewModel>();
        }
    }
}
