﻿using System;
using System.Collections.Generic;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Conditions.Advancement
{
    public class PendingApproveDirectorToDafCondition : IWorkflowConditionState
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting settings;

        public PendingApproveDirectorToDafCondition(IUnitOfWork unitOfWork, AppSetting settings)
        {
            this.unitOfWork = unitOfWork;
            this.settings = settings;
        }

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            var advancement = (Domain.Models.AdvancementAndRefund.Advancement)entity;

            var value = GetValueSetting(advancement.CurrencyId);

            if (advancement.Ammount < value)
            {
                return true;
            }

            return false;
        }

        private int GetValueSetting(int currencyId)
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
