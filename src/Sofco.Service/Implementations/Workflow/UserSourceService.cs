using System;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.Workflow;

namespace Sofco.Service.Implementations.Workflow
{
    public class UserSourceService : IUserSourceService
    {
        private readonly ILogMailer<UserSourceService> logger;
        private readonly IUnitOfWork unitOfWork;

        public UserSourceService(IUnitOfWork unitOfWork,
            ILogMailer<UserSourceService> logger)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public UserSource Get(string code)
        {
            var domain = unitOfWork.UserSourceRepository.Get(code, 0) ?? AddUserSource(code, 0);

            return domain;
        }

        public UserSource Get(string code, int sourceId)
        {
            var domain = unitOfWork.UserSourceRepository.Get(code, sourceId) ?? AddUserSource(code, sourceId);

            return domain;
        }

        private UserSource AddUserSource(string code, int sourceId)
        {
            var newUserSource = new UserSource { Code = code, SourceId = sourceId };

            try
            {
                unitOfWork.UserSourceRepository.Add(newUserSource);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }

            return newUserSource;
        }
    }
}
