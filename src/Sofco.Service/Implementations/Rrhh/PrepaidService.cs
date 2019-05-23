using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Rrhh
{
    public class PrepaidService : IPrepaidService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<PrepaidService> logger;
        private readonly IPrepaidFactory prepaidFactory;

        public PrepaidService(IUnitOfWork unitOfWork, ILogMailer<PrepaidService> logger, IPrepaidFactory prepaidFactory)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.prepaidFactory = prepaidFactory;
        }

        public Response Import(int prepaidId, int yearId, int monthId, IFormFile file)
        {
            var response = new Response();

            var prepaid = unitOfWork.UtilsRepository.GetPrepaid(prepaidId);

            if (prepaid == null)
                response.AddError(Resources.Rrhh.Prepaid.NotFound);

            if(monthId < 1 || monthId > 12)
                response.AddError(Resources.Rrhh.Prepaid.MonthError);

            var today = DateTime.UtcNow;

            if(yearId < today.AddYears(-1).Year || yearId > today.Year)
                response.AddError(Resources.Rrhh.Prepaid.YearError);

            if (response.HasErrors()) return response;

            try
            {
                var fileManager = prepaidFactory.GetInstance(prepaid?.Code);

                if (fileManager == null)
                {
                    response.AddError(Resources.Rrhh.Prepaid.NotImplemented);
                    return response;
                }

                response = fileManager.Process(yearId, monthId, file);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.SaveFileError);
            }

            return response;
        }

        public Response<IList<PrepaidDashboard>> GetDashboard(int yearId, int monthId)
        {
            var response = new Response<IList<PrepaidDashboard>>();

            response.Data = unitOfWork.PrepaidImportedDataRepository.GetDashboard(yearId, monthId);

            return response;
        }
    }
}
