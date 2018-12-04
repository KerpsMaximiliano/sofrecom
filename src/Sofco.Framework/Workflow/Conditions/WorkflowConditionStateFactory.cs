using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;

namespace Sofco.Framework.Workflow.Conditions
{
    public class WorkflowConditionStateFactory : IWorkflowConditionStateFactory
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly AppSetting settings;

        public WorkflowConditionStateFactory(IUnitOfWork unitOfWork, IUserData userData, IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.settings = settingOptions.Value;
        }

        public IWorkflowConditionState GetInstance(string code)
        {
            switch (code)
            {
                case "PENDING-APPROVE-MANAGER-TO-DIRECTOR": return new PendingApproveManagerToDirectorCondition(unitOfWork, userData, this.settings);
                case "PENDING-APPROVE-MANAGER-TO-RRHH": return new PendingApproveManagerToRrhhCondition(unitOfWork, userData, this.settings);
                default: return null;
            }
        }
    }
}
