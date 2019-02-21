using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Validations.Workflow
{
    public class WorkflowValidation : IWorkflowValidation
    {
        private readonly IUnitOfWork unitOfWork;

        public WorkflowValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void ValidateAdd(WorkflowAddModel model, Response response)
        {
            ValidateType(model, response);
            ValidateDescription(model, response);
            ValidateVersion(model, response);
        }

        public void ValidateUpdate(WorkflowAddModel model, Response response)
        {
            ValidateDescription(model, response);
            ValidateVersion(model, response);
        }

        private void ValidateVersion(WorkflowAddModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.Version))
            {
                response.AddError(Resources.Workflow.Workflow.VersionRequired);
            }
        }

        private void ValidateDescription(WorkflowAddModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                response.AddError(Resources.Workflow.Workflow.DescriptionRequired);
            }
        }

        private void ValidateType(WorkflowAddModel model, Response response)
        {
            if (!model.Type.HasValue || model.Type == 0)
            {
                response.AddError(Resources.Workflow.Workflow.TypeRequired);
            }
            else if (!unitOfWork.WorkflowRepository.TypeExist(model.Type.Value))
            {
                response.AddError(Resources.Workflow.Workflow.TypeNotFound);
            }
        }
    }
}
