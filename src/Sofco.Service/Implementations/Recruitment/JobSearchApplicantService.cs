using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Recruitment
{
    public class JobSearchApplicantService : IJobSearchApplicantService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<JobSearchApplicantService> logger;
        private readonly IUserData userData;

        public JobSearchApplicantService(IUnitOfWork unitOfWork, ILogMailer<JobSearchApplicantService> logger, IUserData userData)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public Response<IList<JobSearchApplicantModel>> GetByJobSearch(int jobSearchId)
        {
            var response = new Response<IList<JobSearchApplicantModel>> { Data = new List<JobSearchApplicantModel>() };

            var jobSearch = unitOfWork.JobSearchRepository.GetWithProfilesAndSkills(jobSearchId);

            if (jobSearch == null)
            {
                response.AddError(Resources.Recruitment.JobSearch.NotFound);
                return response;
            }

            try
            {
                var skills = jobSearch.JobSearchSkillsRequired.Select(x => x.SkillId).ToList();
                var profiles = jobSearch.JobSearchProfiles.Select(x => x.ProfileId).ToList();

                var applicants = unitOfWork.JobSearchApplicantRepository.Get(skills, profiles);

                response.Data = applicants.Select(x => new JobSearchApplicantModel(x)).ToList();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }

            return response;
        }

        public Response Add(JobSearchApplicantAddModel model)
        {
            var response = new Response();

            if (!model.ReasonId.HasValue)
                response.AddError(Resources.Recruitment.JobSearch.ReasonCauseRequired);

            if(string.IsNullOrWhiteSpace(model.Comments))
                response.AddError(Resources.Recruitment.JobSearch.CommentsRequired);

            if (response.HasErrors()) return response;

            var currentUser = userData.GetCurrentUser();

            try
            {
                foreach (var applicantId in model.Applicants)
                {
                    var itemToAdd = new JobSearchApplicant
                    {
                        ApplicantId = applicantId,
                        Comments = model.Comments,
                        JobSearchId = model.JobSearchId,
                        ReasonId = model.ReasonId.GetValueOrDefault(),
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = currentUser.UserName
                    };

                    unitOfWork.JobSearchApplicantRepository.Insert(itemToAdd);
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Common.SaveSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }
    }
}
