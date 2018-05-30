using AutoMapper;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.MapProfiles
{
    public class WorkTimeMapProfile : Profile
    {
        public WorkTimeMapProfile()
        {
            CreateMap<WorkTimeApprovalModel, WorkTimeApproval>();
        }
    }
}
