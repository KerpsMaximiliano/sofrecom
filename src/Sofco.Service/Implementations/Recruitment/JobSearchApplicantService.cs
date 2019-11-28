using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Core.Logger;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Common;
using Sofco.Core.Services.Recruitment;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

namespace Sofco.Service.Implementations.Recruitment
{
    public class JobSearchApplicantService : IJobSearchApplicantService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<JobSearchApplicantService> logger;
        private readonly IUserData userData;
        private readonly IOptionRepository<ReasonCause> optionRepository;

        public JobSearchApplicantService(IUnitOfWork unitOfWork, ILogMailer<JobSearchApplicantService> logger, IUserData userData, IOptionRepository<ReasonCause> optionRepository)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.optionRepository = optionRepository;
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

            var reason = optionRepository.Get(model.ReasonId.GetValueOrDefault());

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
               
                if (reason.Type == ReasonCauseType.ApplicantInProgress)
                {
                    foreach (var applicantId in model.Applicants)
                    {
                        var applicant = unitOfWork.ApplicantRepository.Get(applicantId);
                        applicant.Status = ApplicantStatus.InProgress;
                        unitOfWork.ApplicantRepository.Update(applicant);
                    }
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Recruitment.JobSearch.ContactAdded);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response AddInterview(int applicantId, int jobSearchId, InterviewAddModel model)
        {
            var response = new Response();

            var jobSearchApplicant = unitOfWork.JobSearchApplicantRepository.GetById(applicantId, jobSearchId);

            if (jobSearchApplicant == null)
            {
                response.AddError(Resources.Recruitment.JobSearchApplicant.NotFound);
                return response;
            }

            if(model.HasRrhhInterview) ValidateRrhh(model.RrhhInterviewDate, model.RrhhInterviewPlace, model.RrhhInterviewerId, response);
            if(model.HasTechnicalInterview) ValidateRrhh(model.TechnicalInterviewDate, model.TechnicalInterviewPlace, model.TechnicalInterviewerId, response);
            if(model.HasClientInterview) ValidateRrhh(model.ClientInterviewDate, model.ClientInterviewPlace, model.ClientInterviewerId, response);

            if (response.HasErrors()) return response;

            try
            {
                if (model.HasRrhhInterview)
                {
                    jobSearchApplicant.RrhhInterviewDate = model.RrhhInterviewDate;
                    jobSearchApplicant.RrhhInterviewPlace = model.RrhhInterviewPlace;
                    jobSearchApplicant.RrhhInterviewerId = model.RrhhInterviewerId;
                }

                if (model.HasTechnicalInterview)
                {
                    jobSearchApplicant.TechnicalInterviewDate = model.TechnicalInterviewDate;
                    jobSearchApplicant.TechnicalInterviewPlace = model.TechnicalInterviewPlace;
                    jobSearchApplicant.TechnicalInterviewerId = model.TechnicalInterviewerId;
                }

                if (model.HasClientInterview)
                {
                    jobSearchApplicant.ClientInterviewDate = model.ClientInterviewDate;
                    jobSearchApplicant.ClientInterviewPlace = model.ClientInterviewPlace;
                    jobSearchApplicant.ClientInterviewerId = model.ClientInterviewerId;
                }

                jobSearchApplicant.ReasonId = model.ReasonId;

                unitOfWork.JobSearchApplicantRepository.Update(jobSearchApplicant);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void ValidateRrhh(DateTime? date, string place, int? interviewer, Response response)
        {
            if (!date.HasValue)
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewDateRequired);

            if (string.IsNullOrWhiteSpace(place))
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewPlaceRequired);

            if (!interviewer.HasValue || !unitOfWork.UserRepository.ExistById(interviewer.Value))
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewerRequired);
        }
    }
}
