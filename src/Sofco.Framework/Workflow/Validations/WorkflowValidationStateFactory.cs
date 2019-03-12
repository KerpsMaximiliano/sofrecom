using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;

namespace Sofco.Framework.Workflow.Validations
{
    public class WorkflowValidationStateFactory : IWorkflowValidationStateFactory
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;

        public WorkflowValidationStateFactory(IUnitOfWork unitOfWork, IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSettingOptions.Value;
        }

        public IWorkflowValidationState GetInstance(string code)
        {
            switch (code)
            {
                case "REJECT": return new RejectValidationState();
                case "CURRENCY-EXCHANGE": return new CurrencyExchangeValidation(appSetting);
                case "CLOSE-REFUND": return new OnCloseRefundValidation(unitOfWork);
                case "REFUND-ATTACHMENTS": return new RefundAttachmentsValidation(unitOfWork);
                default: return null;
            }
        }
    }
}
