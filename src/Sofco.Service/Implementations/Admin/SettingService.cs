using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class SettingService : ISettingService
    {
        private readonly IGlobalSettingRepository globalParameterRepository;

        public SettingService(IGlobalSettingRepository globalParameterRepository)
        {
            this.globalParameterRepository = globalParameterRepository;
        }

        public Result<List<GlobalSetting>> GetAll()
        {
            return new Result<List<GlobalSetting>>(globalParameterRepository.GetAll());
        }

        public Result Save(List<GlobalSetting> globalParameters)
        {
            globalParameterRepository.Save(globalParameters);

            return new Result(globalParameters);
        }
    }
}
