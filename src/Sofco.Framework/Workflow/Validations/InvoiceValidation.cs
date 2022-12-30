using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class InvoiceValidation : IWorkflowValidationState
    {
        private readonly IUnitOfWork unitOfWork;

        public InvoiceValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            var data = unitOfWork.BuyOrderRepository.GetById(entity.Id);
            if (data?.Invoices == null || !data.Invoices.Any(p => p.FileId.HasValue))
            {
                response.AddError(Resources.RequestNote.BuyOrder.InvoiceRequired);
                return false;
            }
            return true;

        }
    }
}
