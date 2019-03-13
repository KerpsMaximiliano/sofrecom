using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class RefundAttachmentsValidation : IWorkflowValidationState
    {
        private readonly IUnitOfWork unitOfWork;

        public RefundAttachmentsValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            if (!unitOfWork.RefundRepository.HasAttachments(entity.Id))
            {
                response.AddWarning(Resources.AdvancementAndRefund.Refund.HasNoAttachments);
            }    

            return true;
        }
    }
}
