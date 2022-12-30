using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Core.Validations.Workflow;

namespace Sofco.Framework.Workflow.Validations
{
    public class WorkflowValidationStateFactory : IWorkflowValidationStateFactory
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;
        private readonly IRefundValidation validation;

        public WorkflowValidationStateFactory(IUnitOfWork unitOfWork, IOptions<AppSetting> appSettingOptions, IRefundValidation refundValidation)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSettingOptions.Value;
            this.validation = refundValidation;
        }

        public IWorkflowValidationState GetInstance(string code)
        {
            switch (code)
            {
                case "REJECT": return new RejectValidationState();
                case "CURRENCY-EXCHANGE": return new CurrencyExchangeValidation(appSetting, validation, unitOfWork);
                case "CLOSE-REFUND": return new OnCloseRefundValidation(unitOfWork);
                case "REFUND-ATTACHMENTS": return new RefundAttachmentsValidation(unitOfWork);
                case "CASH-RETURN": return new CashReturnValidation(unitOfWork);
                case "ANALYTICS-APPROVED": return new OnAnalyticManagerApprove(unitOfWork);
                case "INVOICE": return new InvoiceValidation(unitOfWork);
                case "PROVIDERS": return new ProvidersValidation(unitOfWork);
                default: return null;
            }
        }
    }
}
