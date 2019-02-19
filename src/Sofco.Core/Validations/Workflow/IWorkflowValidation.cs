using Sofco.Core.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Core.Validations.Workflow
{
    public interface IWorkflowValidation
    {
        void ValidateAdd(WorkflowAddModel model, Response response);
    }
}
