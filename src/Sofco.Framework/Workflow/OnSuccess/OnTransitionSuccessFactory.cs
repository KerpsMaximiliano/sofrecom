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
                case "FINALIZE-ADVANCEMENT-WORKFLOW": return new FinalizeAdvancementWorkflow(workflowManager);
                case "FINALIZE-REFUND-WORKFLOW": return new FinalizeRefundWorkflow(workflowManager, unitOfWork);
                case "CLOSE-REFUND": return new OnCloseRefundSuccess(workflowManager, unitOfWork);
                case "UPLOAD-FILES": return new UploadFilesRequestNote(unitOfWork, appSetting);
                case "CLOSE-REQUEST-NOTE": return new OnCloseBuyOrderSuccess(workflowManager, unitOfWork);
                case "FINALIZE-REQUEST-NOTE": return new FinalizeRefundWorkflow(workflowManager, unitOfWork);
                case "DAF-TO-RECEPTION": return new OnBuyOrderDafToReceptionSuccess(workflowManager, unitOfWork);
                default: return null;
            }
        }
    }
}
