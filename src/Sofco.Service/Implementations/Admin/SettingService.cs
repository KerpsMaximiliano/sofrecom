using System;
using System.Collections.Generic;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Admin;
using Sofco.Framework.ValidationHelpers.Admin;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Admin
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ISettingData settingData;

        private Dictionary<string, Func<Setting, Response<Setting>>> validationDicts;

        public SettingService(IUnitOfWork unitOfWork, ISettingData settingData)
        {
            this.unitOfWork = unitOfWork;
            this.settingData = settingData;

            InitValidations();
        }

        private void InitValidations()
        {
            validationDicts = new Dictionary<string, Func<Setting, Response<Setting>>>
            {
                { "AllocationManagement_Months", SettingValidationHelper.ValidateAllocationManagementMonths },
                { "LicenseCertificatePendingDayOfMonth", SettingValidationHelper.ValidateLicenseCertificatePendingDayOfMonth },
                { "WorkingHoursPerDaysMax", SettingValidationHelper.ValidateWorkingHoursPerDaysMax }
            };
        }

        public Response<List<Setting>> GetAll()
        {
            return new Response<List<Setting>> { Data = unitOfWork.SettingRepository.GetAll() };
        }

        public Response<Setting> GetByKey(string key)
        {
            var response = new Response<Setting>();
            var setting = unitOfWork.SettingRepository.GetSingle(x => x.Key == key);

            if (setting != null)
            {
                response.Data = setting;
                return response;
            }

            response.AddError(Resources.Admin.Setting.SettingNotFound);
            return response;
        }


        public Response<List<Setting>> Save(List<Setting> settings)
        {
            var response = ValidateSettings(settings);

            if (response.HasErrors())
                return response;

            unitOfWork.SettingRepository.Save(settings);

            settingData.ClearKeys();

            response.Data = settings;

            return response;
        }

        public Response<Setting> Save(Setting setting)
        {
            var response = ValidateSetting(setting);

            if (response.HasErrors())
                return response;

            unitOfWork.SettingRepository.Save(setting);
            
            settingData.ClearKeys();

            response.Data = setting;

            return response;
        }

        private Response<Setting> ValidateSetting(Setting setting)
        {
            var response = new Response<Setting>();

            if (!validationDicts.ContainsKey(setting.Key)) return response;

            var validationReponse = validationDicts[setting.Key](setting);

            if (validationReponse.HasErrors())
            {
                response.AddMessages(validationReponse.Messages);
            }

            return response;
        }

        private Response<List<Setting>> ValidateSettings(List<Setting> globalParameters)
        {
            var response = new Response<List<Setting>>();

            foreach (var globalParameter in globalParameters)
            {
                if (!validationDicts.ContainsKey(globalParameter.Key)) continue;

                var validationReponse = validationDicts[globalParameter.Key](globalParameter);

                if (validationReponse.HasErrors())
                {
                    response.AddMessages(validationReponse.Messages);
                }
            }

            return response;
        }

        public IList<LicenseTypeSettingItem> GetLicenseTypes()
        {
            var list = new List<LicenseTypeSettingItem>();
                        
            var licenseTypes = unitOfWork.LicenseTypeRepository.GetAllActivesReadOnly();

            foreach (var licenseType in licenseTypes)
            {
                var item = new LicenseTypeSettingItem
                {
                    TypeId = licenseType.Id,
                    Value = licenseType.Days,
                    Label = licenseType.LabelKey
                };

                if (!string.IsNullOrWhiteSpace(item.Label))
                {
                    list.Add(item);
                }
            }

            return list;
        }

        public Response<List<Setting>> GetJobSettings()
        {
            var data = unitOfWork.SettingRepository.GetByCategory(SettingCategory.Jobs);

            return new Response<List<Setting>> { Data = data };
        }

        public Response Update(Setting settings)
        {
            var reponse = new Response();

            var domain = unitOfWork.SettingRepository.GetByKey(settings.Key);

            if (domain == null)
            {
                reponse.AddError(Resources.Admin.Setting.SettingNotFound);
                return reponse;
            }

            domain.Value = settings.Value;
            unitOfWork.SettingRepository.Update(domain);
            unitOfWork.Save();

            return reponse;
        }
    }
}