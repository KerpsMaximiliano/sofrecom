using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;

namespace Sofco.Framework.Validations.Workflow.Conditions
{
    public class WorkflowConditionStateFactory : IWorkflowConditionStateFactory
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;

        public WorkflowConditionStateFactory(IUnitOfWork unitOfWork, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public IWorkflowConditionState GetInstance(string code)
        {
            switch (code)
            {
                case "PENDING-APPROVE-MANAGER-TO-DIRECTOR": return new PendingApproveManagerToDirectorCondition(unitOfWork, userData);
                case "PENDING-APPROVE-MANAGER-TO-RRHH": return new PendingApproveManagerToRrhhCondition(unitOfWork, userData);
                default: return null;
            }
        }
    }
}
