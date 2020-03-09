using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class FinalizeRefundWorkflow : IOnTransitionSuccessState
    {
        private readonly IWorkflowManager workflowManager;

        private readonly IUnitOfWork unitOfWork;

        public FinalizeRefundWorkflow(IWorkflowManager workflowManager, IUnitOfWork unitOfWork)
        {
            this.workflowManager = workflowManager;
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            this.workflowManager.CloseEntity(entity);

            if(entity is Refund refund && refund.CreditCardId.HasValue) return;

            var tuple = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(entity.Id);

            //Tuple1: Reintegros
            //Tuple2: Adelantos
            // Si diff > 0 tiene mas reintegros que adelantos. Reembolso usuario.
            if (tuple.Item1.Any() && tuple.Item2.Any() && tuple.Item1.All(x => !x.InWorkflowProcess))
            {
                var diff = tuple.Item1.Sum(x => x.TotalAmmount) - tuple.Item2.Sum(x => x.Ammount);

                if (diff == 0)
                {
                    workflowManager.CloseAdvancements(tuple.Item2);
                    workflowManager.CloseRefunds(tuple.Item1);
                }
            }
        }
    }
}
