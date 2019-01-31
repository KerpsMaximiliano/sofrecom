using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Core.Validations.Workflow;
using Sofco.Framework.Workflow.Conditions.Refund;

namespace Sofco.Framework.Workflow.Conditions
{
    public class WorkflowConditionStateFactory : IWorkflowConditionStateFactory
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly AppSetting settings;
        private readonly IRefundValidation validation;

        public WorkflowConditionStateFactory(IUnitOfWork unitOfWork, 
            IUserData userData, 
            IOptions<AppSetting> settingOptions,
            IRefundValidation validation)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.settings = settingOptions.Value;
            this.validation = validation;
        }

        public IWorkflowConditionState GetInstance(string code)
        {
            switch (code)
            {
                case "PENDING-APPROVE-MANAGER-TO-DIRECTOR": return new PendingApproveManagerToDirectorCondition(unitOfWork, userData, settings);
                case "PENDING-APPROVE-MANAGER-TO-DAF": return new PendingApproveManagerToDafCondition(unitOfWork, userData, settings);
                case "PENDING-APPROVE-DIRECTOR-TO-DAF": return new PendingApproveDirectorToDafCondition(unitOfWork, settings);
                case "PENDING-APPROVE-DIRECTOR-TO-DIRECTOR-GENERAL": return new PendingApproveDirectorToGeneralDirectorCondition(unitOfWork, settings);
                case "COMPLIANCE-TO-DIRECTOR": return new ComplianceToDirectorCondition(unitOfWork, settings);
                case "COMPLIANCE-TO-DAF": return new ComplianceToDafCondition(unitOfWork, settings);
                case "DIRECTOR-TO-GENERAL-DIRECTOR": return new DirectorToGeneralDirectorRefundCondition(unitOfWork, settings);
                case "DIRECTOR-TO-DAF": return new DirectorToDafRefundCondition(unitOfWork, settings);
                case "GAF-TO-PAYMENT-PENDING": return new GafToPaymentPendingCondition(validation, unitOfWork);
                case "GAF-TO-FINALIZED": return new GafToFinalizedCondition(validation, unitOfWork);
                default: return null;
            }
        }
    }
}
