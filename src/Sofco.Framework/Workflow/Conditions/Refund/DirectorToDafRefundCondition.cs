﻿using System;
using System.Collections.Generic;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Validations.Workflow;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Workflow.Conditions.Refund
{
    public class DirectorToDafRefundCondition : IWorkflowConditionState
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting settings;

        public DirectorToDafRefundCondition(IUnitOfWork unitOfWork, AppSetting settings)
        {
            this.unitOfWork = unitOfWork;
            this.settings = settings;
        }

        public bool CanDoTransition(WorkflowEntity entity, Response response)
        {
            var refund = (Domain.Models.AdvancementAndRefund.Refund)entity;

            var value = GetValueSetting(refund.CurrencyId);

            if (refund.TotalAmmount < value)
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
