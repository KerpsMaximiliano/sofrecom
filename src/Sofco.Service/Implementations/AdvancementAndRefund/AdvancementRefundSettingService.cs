using System;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class AdvancementRefundSettingService : IAdvancementRefundSettingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<AdvancementRefundSettingService> logger;
        private readonly AppSetting settings;

        public AdvancementRefundSettingService(IUnitOfWork unitOfWork,
            IOptions<AppSetting> settingsOptions,
            ILogMailer<AdvancementRefundSettingService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.settings = settingsOptions.Value;
        }

        public Response<SettingModel> Get()
        {
            var response = new Response<SettingModel> { Data = new SettingModel() };

            var sett1 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountAPesos);
            var sett2 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountADolares);
            var sett3 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountAEuros);
            var sett4 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBPesos);
            var sett5 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBDolares);
            var sett6 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBEuros);

            response.Data.AmmountAPesos = Convert.ToInt32(sett1.Value);
            response.Data.AmmountADolares = Convert.ToInt32(sett2.Value);
            response.Data.AmmountAEuros = Convert.ToInt32(sett3.Value);
            response.Data.AmmountBPesos = Convert.ToInt32(sett4.Value);
            response.Data.AmmountBDolares = Convert.ToInt32(sett5.Value);
            response.Data.AmmountBEuros = Convert.ToInt32(sett6.Value);

            return response;
        }

        public Response Save(SettingModel model)
        {
            var response = new Response();

            if (model == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Setting.ModelError);
                return response;
            }

            try
            {
                var ammounts = new int[]
                {
                    model.AmmountAPesos, model.AmmountADolares, model.AmmountAEuros,
                    model.AmmountBPesos, model.AmmountBDolares, model.AmmountBEuros
                };

                if (ammounts.Any(value => value <= 0))
                {
                    response.AddWarning(Resources.AdvancementAndRefund.Setting.ValuesLessThan0);
                }

                var sett1 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountAPesos);
                var sett2 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountADolares);
                var sett3 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountAEuros);
                var sett4 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBPesos);
                var sett5 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBDolares);
                var sett6 = this.unitOfWork.SettingRepository.GetByKey(settings.AmmountBEuros);

                ValidateAndUpdate(sett1, model.AmmountAPesos);
                ValidateAndUpdate(sett2, model.AmmountADolares);
                ValidateAndUpdate(sett3, model.AmmountAEuros);
                ValidateAndUpdate(sett4, model.AmmountBPesos);
                ValidateAndUpdate(sett5, model.AmmountBDolares);
                ValidateAndUpdate(sett6, model.AmmountBEuros);

                unitOfWork.Save();
                response.AddSuccess(Resources.AdvancementAndRefund.Setting.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void ValidateAndUpdate(Setting setting, int newValue)
        {
            if (Convert.ToInt32(setting.Value) != newValue && newValue > 0)
            {
                setting.Value = newValue.ToString();
                unitOfWork.SettingRepository.Update(setting);
            }
        }
    }
}
