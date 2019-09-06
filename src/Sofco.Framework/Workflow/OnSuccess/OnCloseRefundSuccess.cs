using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnCloseRefundSuccess : IOnTransitionSuccessState
    {
        private readonly IWorkflowManager workflowManager;
        private readonly IUnitOfWork unitOfWork;

        public OnCloseRefundSuccess(IWorkflowManager workflowManager, IUnitOfWork unitOfWork)
        {
            this.workflowManager = workflowManager;
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            var tuple = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(entity.Id);

            //Tuple1: Reintegros
            //Tuple2: Adelantos
            // Si diff > 0 tiene mas reintegros que adelantos. Reembolso usuario.
            if (tuple.Item1.Any() && tuple.Item2.Any())
            {
                var diff = tuple.Item1.Sum(x => x.TotalAmmount) - tuple.Item2.Sum(x => x.Ammount);

                if (diff > 0)
                {
                    workflowManager.CloseAdvancements(tuple.Item2);
                    workflowManager.PayRefunds(tuple.Item1);
                }
            }
        }
    }
}
