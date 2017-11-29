using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Core.DAL;
using Sofco.Core.Services.Admin;
using Sofco.DAL;
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
    }
}
