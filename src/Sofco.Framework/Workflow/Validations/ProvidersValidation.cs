using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class ProvidersValidation : IWorkflowValidationState
    {
        private readonly IUnitOfWork unitOfWork;

        public ProvidersValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            var data = unitOfWork.RequestNoteRepository.GetById(entity.Id);
            if (!data.Providers.Any(p => p.FileId.HasValue))
            {
                response.AddError(Resources.RequestNote.RequestNote.ProvidersRequired);
                return false;
            }
            return true;

        }
    }
}
