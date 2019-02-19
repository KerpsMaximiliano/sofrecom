using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Workflow
{
    public class WorkflowTransitionService : IWorkflowTransitionService
    {
        private readonly IWorkflowTransitionValidation workflowTransitionValidation;

        public WorkflowTransitionService(IWorkflowTransitionValidation workflowTransitionValidation)
        {
            this.workflowTransitionValidation = workflowTransitionValidation;
        }

        public Response Post(WorkflowTransitionAddModel model)
        {
            var response = new Response();

            workflowTransitionValidation.ValidateAdd(model, response);

            if (response.HasErrors()) return response;

            return response;
        }
    }
}
