using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Core.DAL;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork unitOfWork;

        public SettingService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Result<List<GlobalSetting>> GetAll()
        {
            return new Result<List<GlobalSetting>>(unitOfWork.GlobalSettingRepository.GetAll());
        }

        public Result Save(List<GlobalSetting> globalParameters)
        {
            unitOfWork.GlobalSettingRepository.Save(globalParameters);

            return new Result(globalParameters);
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