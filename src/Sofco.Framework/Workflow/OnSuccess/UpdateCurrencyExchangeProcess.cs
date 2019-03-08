using System;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class UpdateCurrencyExchangeProcess : IOnTransitionSuccessState
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting appSetting;

        public UpdateCurrencyExchangeProcess(IUnitOfWork unitOfWork, AppSetting appSetting)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSetting;
        }

        public void Process(WorkflowEntity entity, WorkflowChangeStatusParameters parameters)
        {
            var refund = (Refund) entity;

            if (refund.CurrencyId != appSetting.CurrencyPesos && parameters.Parameters.ContainsKey("currencyExchange"))
            {
                refund.CurrencyExchange = Convert.ToDecimal(parameters.Parameters["currencyExchange"]);
                unitOfWork.RefundRepository.UpdateCurrencyExchange(refund);
                unitOfWork.Save();
            }
        }
    }
}
