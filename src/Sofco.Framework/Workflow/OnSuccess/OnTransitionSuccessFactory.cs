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

        public OnTransitionSuccessFactory(IUnitOfWork unitOfWork, IOptions<AppSetting> appSettingsOptions)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSettingsOptions.Value;
        }

        public IOnTransitionSuccessState GetInstance(string code)
        {
            switch (code)
            {
                case "REJECT": return new OnTransitionRejectSuccess(unitOfWork);
                case "GAF-TO-FINALIZE": return new OnGafToFinalizeSuccess(unitOfWork);
                case "UPDATE-CURRENCY-EXCHANGE": return new UpdateCurrencyExchangeProcess(unitOfWork, appSetting);
                case "FINALIZE-WORKFLOW-PROCESS": return new FinalizeWorkflowProcess(unitOfWork);
                case "CLOSE-REFUND": return new OnCloseRefundSuccess(unitOfWork, this.appSetting);
                default: return null;
            }
        }
    }
}
