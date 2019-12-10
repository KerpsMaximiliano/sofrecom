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
        private readonly FileConfig fileConfig;

        public JobSearchApplicantService(IUnitOfWork unitOfWork, ILogMailer<JobSearchApplicantService> logger, IUserData userData, IOptionRepository<ReasonCause> optionRepository, IOptions<FileConfig> fileOptions)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.optionRepository = optionRepository;
            this.fileConfig = fileOptions.Value;
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

            if(model.HasRrhhInterview) Validate(model.RrhhInterviewDate, model.RrhhInterviewPlace, model.RrhhInterviewerId, response, false);
            if(model.HasTechnicalInterview) Validate(model.TechnicalInterviewDate, model.TechnicalInterviewPlace, model.TechnicalInterviewerId, response, model.IsTechnicalExternal);
            if(model.HasClientInterview) Validate(model.ClientInterviewDate, model.ClientInterviewPlace, model.ClientInterviewerId, response, model.IsClientExternal);

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

                    if (model.IsTechnicalExternal)
                    {
                        jobSearchApplicant.TechnicalExternalInterviewer = model.TechnicalExternalInterviewer;
                        jobSearchApplicant.IsTechnicalExternal = model.IsTechnicalExternal;
                        jobSearchApplicant.TechnicalInterviewerId = null;
                    }
                    else
                    {
                        jobSearchApplicant.TechnicalInterviewerId = model.TechnicalInterviewerId;
                        jobSearchApplicant.IsTechnicalExternal = false;
                        jobSearchApplicant.TechnicalExternalInterviewer = string.Empty;
                    }
                }

                if (model.HasClientInterview)
                {
                    jobSearchApplicant.HasClientInterview = model.HasClientInterview;
                    jobSearchApplicant.ClientInterviewDate = model.ClientInterviewDate;
                    jobSearchApplicant.ClientInterviewPlace = model.ClientInterviewPlace;
                    jobSearchApplicant.ClientInterviewComments = model.ClientInterviewComments;

                    if (model.IsClientExternal)
                    {
                        jobSearchApplicant.ClientExternalInterviewer = model.ClientExternalInterviewer;
                        jobSearchApplicant.IsClientExternal = model.IsClientExternal;
                        jobSearchApplicant.ClientInterviewerId = null;
                    }
                    else
                    {
                        jobSearchApplicant.ClientInterviewerId = model.ClientInterviewerId;
                        jobSearchApplicant.IsClientExternal = false;
                        jobSearchApplicant.ClientExternalInterviewer = string.Empty;
                    }
                }

                jobSearchApplicant.ReasonId = model.ReasonId;

                unitOfWork.JobSearchApplicantRepository.Update(jobSearchApplicant);
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

        public async Task<Response<File>> AttachFile(int applicantId, int jobSearchId, Response<File> response,
            IFormFile file)
        {
            var jobsearchApplicant = unitOfWork.JobSearchApplicantRepository.GetById(applicantId, jobSearchId);

            if (jobsearchApplicant == null)
            {
                response.AddError(Resources.Recruitment.JobSearchApplicant.NotFound);
                return response;
            }

            var user = userData.GetCurrentUser();

            var fileToAdd = new File();
            var lastDotIndex = file.FileName.LastIndexOf('.');

            fileToAdd.FileName = file.FileName;
            fileToAdd.FileType = file.FileName.Substring(lastDotIndex);
            fileToAdd.InternalFileName = Guid.NewGuid();
            fileToAdd.CreationDate = DateTime.UtcNow;
            fileToAdd.CreatedUser = user.UserName;

            var jobsearchApplicantFile = new JobSearchApplicantFile
            {
                File = fileToAdd,
                JobSearchId = jobsearchApplicant.JobSearchId,
                ApplicantId = jobsearchApplicant.ApplicantId,
                Date = jobsearchApplicant.CreatedDate
            };

            try
            {
                var fileName = $"{fileToAdd.InternalFileName.ToString()}{fileToAdd.FileType}";

                using (var fileStream = new FileStream(Path.Combine(fileConfig.RecruitmentPath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                unitOfWork.JobSearchApplicantRepository.InsertFile(jobsearchApplicantFile);
                unitOfWork.Save();

                response.Data = jobsearchApplicantFile.File;
                response.AddSuccess(Resources.Recruitment.JobSearchApplicant.FileAdded);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.SaveFileError);
                logger.LogError(e);
            }

            return response;
        }

        private void Validate(DateTime? date, string place, int? interviewer, Response response,
            bool isExternal)
        {
            if (!date.HasValue)
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewDateRequired);

            if (string.IsNullOrWhiteSpace(place))
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewPlaceRequired);

            if (!isExternal && (!interviewer.HasValue || !unitOfWork.UserRepository.ExistById(interviewer.Value)))
                response.AddError(Resources.Recruitment.JobSearchApplicant.InterviewerRequired);
        }
    }
}
