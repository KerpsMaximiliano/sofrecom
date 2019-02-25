﻿using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;

namespace Sofco.Framework.Workflow.OnSuccess
{
    public class OnTransitionSuccessFactory : IOnTransitionSuccessFactory
    {
        private readonly IUnitOfWork unitOfWork;

        public OnTransitionSuccessFactory(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IOnTransitionSuccessState GetInstance(string code)
        {
            switch (code)
            {
                case "REJECT": return new OnTransitionRejectSuccess(unitOfWork);
                case "GAF-TO-FINALIZE": return new OnGafToFinalizeSuccess(unitOfWork);
                case "PAYMENT-PENDING-TO-FINALIZE": return new OnPaymentPendingToFinalized(unitOfWork);
                default: return null;
            }
        }
    }
}
