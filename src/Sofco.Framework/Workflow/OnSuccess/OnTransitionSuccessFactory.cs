using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnTransitionSuccessFactory : IOnTransitionSuccessFactory
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;
        private readonly IWorkflowManager workflowManager;

        public OnTransitionSuccessFactory(IUnitOfWork unitOfWork, IOptions<AppSetting> appSettingsOptions, IWorkflowManager workflowManager)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSettingsOptions.Value;
            this.workflowManager = workflowManager;
        }

        public IOnTransitionSuccessState GetInstance(string code)
        {
            switch (code)
            {
                case "REJECT": return new OnTransitionRejectSuccess(unitOfWork);
                case "GAF-TO-FINALIZE": return new OnGafToFinalizeSuccess(unitOfWork);
                case "UPDATE-CURRENCY-EXCHANGE": return new UpdateCurrencyExchangeProcess(unitOfWork, appSetting);
                case "FINALIZE-WORKFLOW-PROCESS": return new FinalizeWorkflowProcess(workflowManager);
                case "CLOSE-REFUND": return new OnCloseRefundSuccess(workflowManager);
                case "CLOSE-CASH-RETURN": return new CloseCashReturnProcess(workflowManager, unitOfWork);
                default: return null;
            }
        }
    }
}
