using AutoMapper;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Models.Workflow;

namespace Sofco.Service.MapProfiles
{
    public class WorkflowMapProfile : Profile
    {
        public WorkflowMapProfile()
        {
            CreateMap<WorkflowState, WorkflowStateOptionModel>()
                .ForMember(s => s.Text, x => x.MapFrom(_ => _.Name));
        }
    }
}
