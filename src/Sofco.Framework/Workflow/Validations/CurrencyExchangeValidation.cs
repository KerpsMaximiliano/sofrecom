using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Validations
{
    public class CurrencyExchangeValidation : IWorkflowValidationState
    {
        private readonly AppSetting appSetting;

        private readonly IRefundValidation validation;

        private readonly IUnitOfWork unitOfWork;

        public CurrencyExchangeValidation(AppSetting appSetting, IRefundValidation validation, IUnitOfWork unitOfWork)
        {
            this.appSetting = appSetting;
            this.validation = validation;
            this.unitOfWork = unitOfWork;
        }

        public bool Validate(WorkflowEntity entity, Response response, WorkflowChangeStatusParameters parameters)
        {
            if (entity is Refund refund)
            {
                var domain = unitOfWork.RefundRepository.GetFullById(entity.Id);

                if (refund.CurrencyId != appSetting.CurrencyPesos && !parameters.Parameters.ContainsKey("currencyExchange"))
                {
                    if (validation.HasUserRefund(domain))
                    {
                        response.AddError(Resources.Workflow.Workflow.CurrencyExchangeRequired);
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
