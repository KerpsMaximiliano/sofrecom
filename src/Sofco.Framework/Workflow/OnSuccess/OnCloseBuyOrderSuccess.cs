using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnCloseBuyOrderSuccess : IOnTransitionSuccessState
    {
        private readonly IWorkflowManager workflowManager;
        private readonly IUnitOfWork unitOfWork;
        //Cuando recibimos mercadería (en una OC), tiene que intentar cerrar la NP padre
        //validando que complete el total de la NP (si la completó, cierra, si no, queda/pasa a recepción parcial)
        public OnCloseBuyOrderSuccess(IWorkflowManager workflowManager, IUnitOfWork unitOfWork)
        {
            this.workflowManager = workflowManager;
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            var order = unitOfWork.BuyOrderRepository.GetById(entity.Id);
            var isCompleted = unitOfWork.RequestNoteRepository.IsCompletelyDelivered(order.RequestNoteId);
            var note = unitOfWork.RequestNoteRepository.GetById(order.RequestNoteId);
            if (isCompleted)
            {
                workflowManager.CloseRequestNote(note);
            }
            else
            {
                workflowManager.PartialReceptionRequestNote(note);
            }
            
        }
    }
}
