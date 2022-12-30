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
using Sofco.Domain.Models.RequestNote;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class FinalizeBuyOrderWorkflow : IOnTransitionSuccessState
    {
        private readonly IWorkflowManager workflowManager;

        private readonly IUnitOfWork unitOfWork;

        public FinalizeBuyOrderWorkflow(IWorkflowManager workflowManager, IUnitOfWork unitOfWork)
        {
            this.workflowManager = workflowManager;
            this.unitOfWork = unitOfWork;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            this.workflowManager.CloseEntity(entity);
        }
    }
}
