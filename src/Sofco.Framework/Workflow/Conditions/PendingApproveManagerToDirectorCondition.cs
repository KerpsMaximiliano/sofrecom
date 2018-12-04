using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Conditions
{
    public class PendingApproveManagerToDirectorCondition : IWorkflowConditionState
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly AppSetting settings;

        public PendingApproveManagerToDirectorCondition(IUnitOfWork unitOfWork, IUserData userData, AppSetting settings)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.settings = settings;
        }

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            var currentUser = userData.GetCurrentUser();

            var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

            var sectors = unitOfWork.EmployeeRepository.GetAnalyticsWithSector(employee.Id);

            var advancement = (Advancement)entity;

            var value = GetValueSetting(advancement.CurrencyId);

            if (advancement.Ammount > value)
            {
                if (sectors.All(x => x.ResponsableUserId != entity.UserApplicantId && x.ResponsableUserId != currentUser.Id))
                {
                    return true;
                }
            }

            return false;
        }

        private int GetValueSetting(int currencyId)
        {
            var dictionary = new Dictionary<int, Func<int>>();

            dictionary.Add(settings.CurrencyPesos, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey("AdvancementAmmountAPesos");
                return Convert.ToInt32(sett.Value);
            });

            dictionary.Add(settings.CurrencyDolares, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey("AdvancementAmmountADolares");
                return Convert.ToInt32(sett.Value);
            });

            dictionary.Add(settings.CurrencyEuros, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey("AdvancementAmmountAEuros");
                return Convert.ToInt32(sett.Value);
            });

            return dictionary.ContainsKey(currencyId) ? dictionary[currencyId].Invoke() : 0;
        }
    }
}
