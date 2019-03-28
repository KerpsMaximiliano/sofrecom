using Sofco.Common.Settings;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Conditions.Advancement
{
    public class IsNotForeignCurrencyCondition : IWorkflowConditionState
    {
        private readonly AppSetting settings;

        public IsNotForeignCurrencyCondition(AppSetting settings)
        {
            this.settings = settings;
        }

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            var advancement = (Domain.Models.AdvancementAndRefund.Advancement)entity;

            return advancement.CurrencyId == settings.CurrencyPesos;
        }
    }
}
