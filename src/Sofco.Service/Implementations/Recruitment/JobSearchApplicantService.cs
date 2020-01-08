using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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
using File = Sofco.Domain.Models.Common.File;
using Sofco.Core.Config;

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
                var date = DateTime.UtcNow.Date;
                var anySuccess = false;

                foreach (var jobSearchId in model.JobSearchs)
                {
                    foreach (var applicantId in model.Applicants)
                    {
                        if (!unitOfWork.JobSearchApplicantRepository.Exist(applicantId, jobSearchId, date))
                        {
                            var itemToAdd = new JobSearchApplicant
                            {
                                ApplicantId = applicantId,
                                Comments = model.Comments,
                                JobSearchId = jobSearchId,
                                ReasonId = model.ReasonId.GetValueOrDefault(),
                                CreatedDate = date,
                                CreatedBy = currentUser.UserName
                            };

                            unitOfWork.JobSearchApplicantRepository.Insert(itemToAdd);
                            anySuccess = true;
                        }
                        else
                        {
                            var applicant = unitOfWork.ApplicantRepository.Get(applicantId);
                            response.AddWarningAndNoTraslate($"Ya existe un contacto para el postulante {applicant.FirstName} {applicant.LastName} para la busqueda #{jobSearchId} con fecha de hoy");   
                        }
                    }
                }
               
                if (anySuccess)
                {
                    if (reason.Type == ReasonCauseType.ApplicantInProgress ||
                        reason.Type == ReasonCauseType.ApplicantContacted)
                    {
                        foreach (var applicantId in model.Applicants)
                        {
                            var applicant = unitOfWork.ApplicantRepository.Get(applicantId);
                            applicant.Status = reason.Type == ReasonCauseType.ApplicantInProgress ? ApplicantStatus.InProgress : ApplicantStatus.Contacted;
                            unitOfWork.ApplicantRepository.Update(applicant);
                        }
                    }
                }

                if (anySuccess)
                {
                    unitOfWork.Save();
                    response.AddSuccess(Resources.Recruitment.JobSearch.ContactAdded);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<int> AddInterview(int applicantId, int jobSearchId, DateTime date, InterviewAddModel model)
        {
            var response = new Response<int>();

            var jobSearchApplicant = unitOfWork.JobSearchApplicantRepository.GetById(applicantId, jobSearchId, date);

            if (jobSearchApplicant == null)
            {
                response.AddError(Resources.Recruitment.JobSearchApplicant.NotFound);
                return response;
            }

            if(model.HasRrhhInterview) Validate(model.RrhhInterviewDate, model.RrhhInterviewPlace, model.RrhhInterviewerId.ToString(), response);
            if(model.HasTechnicalInterview) Validate(model.TechnicalInterviewDate, model.TechnicalInterviewPlace, model.TechnicalExternalInterviewer, response);
            if(model.HasClientInterview) Validate(model.ClientInterviewDate, model.ClientInterviewPlace, model.ClientExternalInterviewer, response);

            if (model.HasRrhhInterview && (!model.Salary.HasValue || model.Salary <= 0))
            {
                response.AddError(Resources.Recruitment.JobSearchApplicant.SalaryRequired);
            }

            if (response.HasErrors()) return response;

            try
            {
                if (model.HasRrhhInterview)
                {
                    jobSearchApplicant.HasRrhhInterview = model.HasRrhhInterview;
                    jobSearchApplicant.RrhhInterviewDate = model.RrhhInterviewDate;
                    jobSearchApplicant.RrhhInterviewPlace = model.RrhhInterviewPlace;
                    jobSearchApplicant.RrhhInterviewerId = model.RrhhInterviewerId;
                    jobSearchApplicant.RrhhInterviewComments = model.RrhhInterviewComments;
                }

                if (model.HasTechnicalInterview)
                {
                    jobSearchApplicant.HasTechnicalInterview = model.HasTechnicalInterview;
                    jobSearchApplicant.TechnicalInterviewDate = model.TechnicalInterviewDate;
                    jobSearchApplicant.TechnicalInterviewPlace = model.TechnicalInterviewPlace;
                    jobSearchApplicant.TechnicalInterviewComments = model.TechnicalInterviewComments;
                    jobSearchApplicant.TechnicalExternalInterviewer = model.TechnicalExternalInterviewer;
                }

                if (model.HasClientInterview)
                {
                    jobSearchApplicant.HasClientInterview = model.HasClientInterview;
                    jobSearchApplicant.ClientInterviewDate = model.ClientInterviewDate;
                    jobSearchApplicant.ClientInterviewPlace = model.ClientInterviewPlace;
                    jobSearchApplicant.ClientInterviewComments = model.ClientInterviewComments;
                    jobSearchApplicant.ClientExternalInterviewer = model.ClientExternalInterviewer;
                }

                jobSearchApplicant.ReasonId = model.ReasonId;
                jobSearchApplicant.RemoteWork = model.RemoteWork;
                jobSearchApplicant.Salary = model.Salary;

                unitOfWork.JobSearchApplicantRepository.Update(jobSearchApplicant);
                unitOfWork.Save();

                response.AddSuccess(Resources.Common.SaveSuccess);

                response.Data = -1;

                var reason = optionRepository.Get(model.ReasonId);

                if (reason.Type == ReasonCauseType.ApplicantContacted || reason.Type == ReasonCauseType.ApplicantOpen)
                {
                    var jobsearchs = unitOfWork.JobSearchApplicantRepository.GetByApplicant(applicantId);

                    if (jobsearchs.All(x => x.Reason.Type != ReasonCauseType.ApplicantInProgress))
                    {
                        var applicant = unitOfWork.ApplicantRepository.Get(applicantId);

                        if (reason.Type == ReasonCauseType.ApplicantContacted)
                            applicant.Status = ApplicantStatus.Contacted;

                        if (reason.Type == ReasonCauseType.ApplicantOpen)
                            applicant.Status = ApplicantStatus.Valid;

                        unitOfWork.ApplicantRepository.Update(applicant);
                        unitOfWork.Save();

                        response.Data = Convert.ToInt32(applicant.Status);
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<ApplicantJobSearchModel>> GetByApplicant(int applicantId)
        {
            var response = new Response<IList<ApplicantJobSearchModel>> { Data = new List<ApplicantJobSearchModel>() };

            var applicant = unitOfWork.ApplicantRepository.GetWithProfilesAndSkills(applicantId);

            if (applicant == null)
            {
                response.AddError(Resources.Recruitment.JobSearch.NotFound);
                return response;
            }

            try
            {
                var skills = applicant.ApplicantSkills.Select(x => x.SkillId).ToList();
                var profiles = applicant.ApplicantProfiles.Select(x => x.ProfileId).ToList();

                var jobSearchs = unitOfWork.JobSearchRepository.Get(skills, profiles);

                response.Data = jobSearchs.Select(x => new ApplicantJobSearchModel(x)).OrderBy(x => x.ContactedBefore).ToList();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }

            return response;
        }

        private void Validate(DateTime? date, string place, string interviewer, Response response)
        {
            if (!date.HasValue)
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewDateRequired);
            else
            {
                if (date.Value.Date < DateTime.UtcNow.Date)
                {
                    response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewDateLessThanToday);
                }
            }

            if (string.IsNullOrWhiteSpace(place))
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewPlaceRequired);

            if (string.IsNullOrWhiteSpace(interviewer))
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewerRequired);
        }
    }
}
