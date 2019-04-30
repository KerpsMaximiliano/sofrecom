using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Conditions.Advancement
{
    public class ManagerToGeneralDirectorCondition : IWorkflowConditionState
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly AppSetting settings;

        public ManagerToGeneralDirectorCondition(IUnitOfWork unitOfWork, IUserData userData, AppSetting settings)
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

            var advancement = (Domain.Models.AdvancementAndRefund.Advancement)entity;

            var value = GetValueSetting(advancement.CurrencyId);

            if (advancement.Ammount >= value)
            {
                if (sectors.Any(x => x.ResponsableUserId == entity.UserApplicantId || x.ResponsableUserId == currentUser.Id))
                {
                    var valueB = GetValueSettingB(advancement.CurrencyId);

                    return advancement.Ammount >= valueB;
                }
            }

            return false;
        }

        private int GetValueSetting(int currencyId)
        {
            var dictionary = new Dictionary<int, Func<int>>();

            dictionary.Add(settings.CurrencyPesos, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountAPesos);
                return Convert.ToInt32(sett.Value);
            });

            dictionary.Add(settings.CurrencyDolares, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountADolares);
                return Convert.ToInt32(sett.Value);
            });

            dictionary.Add(settings.CurrencyEuros, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountAEuros);
                return Convert.ToInt32(sett.Value);
            });

            return dictionary.ContainsKey(currencyId) ? dictionary[currencyId].Invoke() : 0;
        }

        private int GetValueSettingB(int currencyId)
        {
            var dictionary = new Dictionary<int, Func<int>>();

            dictionary.Add(settings.CurrencyPesos, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBPesos);
                return Convert.ToInt32(sett.Value);
            });

            dictionary.Add(settings.CurrencyDolares, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBDolares);
                return Convert.ToInt32(sett.Value);
            });

            dictionary.Add(settings.CurrencyEuros, () =>
            {
                var sett = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBEuros);
                return Convert.ToInt32(sett.Value);
            });

            return dictionary.ContainsKey(currencyId) ? dictionary[currencyId].Invoke() : 0;
        }
    }
}
