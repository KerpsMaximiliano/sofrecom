using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Recruitment
{
    public class RecruitmentReportService : IRecruitmentReportService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RecruitmentReportService> logger;
        private readonly IUserData userData;
        private readonly IRoleManager roleManager;

        public RecruitmentReportService(IUnitOfWork unitOfWork, ILogMailer<RecruitmentReportService> logger, IUserData userData, IRoleManager roleManager)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.roleManager = roleManager;
        }

        public Response<IList<RecruitmentReportResponse>> Search(RecruitmentReportParameters parameter)
        {
            var response = new Response<IList<RecruitmentReportResponse>> { Data = new List<RecruitmentReportResponse>() };

            try
            {
                var list = unitOfWork.JobSearchRepository.SearchForReport(parameter);

                if (roleManager.IsRrhh())
                {
                    response.Data = list.Select(x => new RecruitmentReportResponse(x)).ToList();
                }
                else
                {
                    var currentUser = userData.GetCurrentUser();
                    response.Data = list.Where(x => x.UserId == currentUser.Id).Select(x => new RecruitmentReportResponse(x)).ToList();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }

            return response;
        }
    }
}
