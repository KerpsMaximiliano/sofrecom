using Sofco.Common.Settings;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class CurrencyExchangeValidation : IWorkflowValidationState
    {
        private readonly AppSetting appSetting;

        public CurrencyExchangeValidation(AppSetting appSetting)
        {
            this.appSetting = appSetting;
        }

        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            if (entity is Refund refund)
            {
                if (refund.CurrencyId != appSetting.CurrencyPesos && !parameters.Parameters.ContainsKey("currencyExchange"))
                {
                    response.AddError(Resources.Workflow.Workflow.CurrencyExchangeRequired);
                    return false;
                }
            }

            return true;
        }
    }
}
