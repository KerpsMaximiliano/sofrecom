using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Admin;
using Sofco.Framework.ValidationHelpers.Admin;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Admin
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork unitOfWork;

        private Dictionary<string, Func<Setting, Response<Setting>>> validationDicts;

        public SettingService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            InitValidations();
        }

        private void InitValidations()
        {
            validationDicts = new Dictionary<string, Func<Setting, Response<Setting>>>
            {
                {"AllocationManagement_Months", SettingValidationHelper.ValidateAllocationManagementMonths}
            };
        }

        public Response<List<Setting>> GetAll()
        {
            return new Response<List<Setting>> { Data = unitOfWork.SettingRepository.GetAll() };
        }

        public Response<List<Setting>> Save(List<Setting> globalParameters)
        {
            var response = ValidateSettings(globalParameters);

            if (response.HasErrors())
                return response;

            unitOfWork.SettingRepository.Save(globalParameters);

            response.Data = globalParameters;

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

            var licenseTypes = unitOfWork.LicenseTypeRepository.GetAllReadOnly();

            foreach (var licenseType in licenseTypes)
            {
                var item = new LicenseTypeSettingItem
                {
                    TypeId = licenseType.Id,
                    Value = licenseType.Days
                };

                switch (licenseType.Id)
                {
                    case 7: item.Label = "rrhh.license.exam"; break;
                    case 8: item.Label = "rrhh.license.marrige"; break;
                    case 10: item.Label = "rrhh.license.born"; break;
                    case 11: item.Label = "rrhh.license.deadParent"; break;
                    case 15: item.Label = "rrhh.license.deadBrother"; break;
                }

                if (!string.IsNullOrWhiteSpace(item.Label))
                {
                    list.Add(item);
                }
            }

            return list;
        }
    }
}