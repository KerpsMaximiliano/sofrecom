using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnBuyOrderDafToReceptionSuccess : IOnTransitionSuccessState
    {
        private readonly IWorkflowManager workflowManager;
        private readonly IUnitOfWork unitOfWork;
        //Cuando se cambia el estado de la OC a Pendiente Recepción Mercaderia => Se cambia el estado de la NP a Pendiente Recepción Mercaderia.
        public OnBuyOrderDafToReceptionSuccess(IWorkflowManager workflowManager, IUnitOfWork unitOfWork)
        {
            this.workflowManager = workflowManager;
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            if (parameters is WorkflowChangeBuyOrderParameters)
            {
                var order = unitOfWork.BuyOrderRepository.GetById(entity.Id);
                var note = unitOfWork.RequestNoteRepository.GetById(order.RequestNoteId);

                //Acá realmente validamos en cuál debe quedar y lo devolvemos al workflow original, para que desde allá vea si
                //hay algo, invoca al workflow de NP (si es null, por ej si ya tiene el estado al cual debería pasar, no hace nada)
                //workflowManager.CloseRequestNote(note);
                ((WorkflowChangeBuyOrderParameters)parameters).NextStateIdRequestNote = workflowManager.PendingReceptionRequestNote(note, order);

            }
        }
    }
}
