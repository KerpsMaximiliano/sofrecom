using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class OnAnalyticManagerApprove : IWorkflowValidationState
    {
        private readonly IUnitOfWork unitOfWork;

        public OnAnalyticManagerApprove(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            var data = unitOfWork.RequestNoteAnalitycRepository.GetByRequestNoteId(entity.Id);
            return data.All(a => a.Status == "Aprobada");

        }
    }
}
