﻿using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Validations.Workflow
{
    public class WorkflowTransitionValidation : IWorkflowTransitionValidation
    {
        private readonly IUnitOfWork unitOfWork;

        public WorkflowTransitionValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void ValidateAdd(WorkflowTransitionAddModel model, Response response, int transitionId = 0)
        {
            if (model == null)
            {
                response.AddError(Resources.Workflow.Workflow.TransitionParametersNull);
                return;
            }

            ValidateActualState(model, response);
            ValidateNextState(model, response);
            ValidateWorkflow(model, response);
            ValidateStates(model, response);

            if (response.HasErrors()) return;

            ValidateIfTransitionExist(model, response, transitionId);
        }

        private void ValidateIfTransitionExist(WorkflowTransitionAddModel model, Response response, int transitionId)
        {
            if (unitOfWork.WorkflowRepository.TransitionExist(transitionId,
                model.ActualWorkflowStateId.GetValueOrDefault(), model.NextWorkflowStateId.GetValueOrDefault(), 
                model.WorkflowId.GetValueOrDefault()))
            {
                response.AddError(Resources.Workflow.Workflow.TransitionAlreadyExist);
            }
        }

        private void ValidateStates(WorkflowTransitionAddModel model, Response response)
        {
            if (model.ActualWorkflowStateId.HasValue && model.NextWorkflowStateId.HasValue)
            {
                if (model.ActualWorkflowStateId == model.NextWorkflowStateId)
                {
                    response.AddError(Resources.Workflow.Workflow.StatesEquals);
                }
            }
        }

        private void ValidateWorkflow(WorkflowTransitionAddModel model, Response response)
        {
            if (!model.WorkflowId.HasValue || model.WorkflowId.Value == 0)
            {
                response.AddError(Resources.Workflow.Workflow.WorkflowIsRequired);
            }
            else if (!unitOfWork.WorkflowRepository.WorkflowExist(model.WorkflowId.Value))
            {
                response.AddError(Resources.Workflow.Workflow.WorkflowNotFound);
            }
        }

        private void ValidateNextState(WorkflowTransitionAddModel model, Response response)
        {
            if (!model.NextWorkflowStateId.HasValue || model.NextWorkflowStateId.Value == 0)
            {
                response.AddError(Resources.Workflow.Workflow.NextStateIsRequired);
            }
            else if (!unitOfWork.WorkflowRepository.StateExist(model.NextWorkflowStateId.Value))
            {
                response.AddError(Resources.Workflow.Workflow.NextStateNotFound);
            }
        }

        private void ValidateActualState(WorkflowTransitionAddModel model, Response response)
        {
            if (!model.ActualWorkflowStateId.HasValue || model.ActualWorkflowStateId.Value == 0)
            {
                response.AddError(Resources.Workflow.Workflow.ActualStateIsRequired);
            }
            else if (!unitOfWork.WorkflowRepository.StateExist(model.ActualWorkflowStateId.Value))
            {
                response.AddError(Resources.Workflow.Workflow.ActualStateNotFound);
            }
        }
    }
}
