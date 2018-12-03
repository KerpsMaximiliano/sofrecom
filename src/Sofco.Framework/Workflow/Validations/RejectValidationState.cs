using System;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class RejectValidationState : IWorkflowValidationState
    {
        public bool Validate(WorkflowEntity entity, Response response)
        {
            throw new NotImplementedException();
        }
    }
}
