using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Common;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Recruitment
{
    public class JobSearchService : IJobSearchService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<JobSearchService> logger;
        private readonly IUserData userData;
        private readonly EmailConfig emailConfig;

        public JobSearchService(IUnitOfWork unitOfWork, ILogMailer<JobSearchService> logger, IUserData userData, IOptions<EmailConfig> emailConfigOptions)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.emailConfig = emailConfigOptions.Value;
        }

        public Response Add(JobSearchAddModel model)
        {
            var response = new Response();

            if (model == null)
            {
                response.AddError(Resources.Recruitment.JobSearch.ModelNull);
                return response;
            }

            ValidateClient(model, response);
            ValidateMaximunSalary(model, response);
            ValidateQuantity(model, response);
            ValidateReasonCause(model, response);
            ValidateUser(model, response);
            ValidateSelector(model, response);
            ValidateTimeHiring(model, response);
            ValidateLanguage(model, response);
            ValidateStudy(model, response);
            ValidateMarketStudy(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain();
                domain.CreatedDate = DateTime.UtcNow.Date;
                domain.CreatedBy = userData.GetCurrentUser().UserName;

                unitOfWork.JobSearchRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Recruitment.JobSearch.AddSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Update(int id, JobSearchAddModel model)
        {
            var response = new Response();

            if (model == null)
            {
                response.AddError(Resources.Recruitment.JobSearch.ModelNull);
                return response;
            }

            var jobsearch = unitOfWork.JobSearchRepository.GetWithProfilesAndSkillsAndSenorities(id);

            if (jobsearch == null)
            {
                response.AddError(Resources.Recruitment.JobSearch.NotFound);
                return response;
            }

            ValidateClient(model, response);
            ValidateMaximunSalary(model, response);
            ValidateQuantity(model, response);
            ValidateReasonCause(model, response);
            ValidateUser(model, response);
            ValidateSelector(model, response);
            ValidateTimeHiring(model, response);
            ValidateLanguage(model, response);
            ValidateStudy(model, response);
            ValidateMarketStudy(model, response);

            if (response.HasErrors()) return response;

            try
            {
                model.UpdateDomain(jobsearch);

                unitOfWork.JobSearchRepository.Update(jobsearch);
                unitOfWork.Save();

                response.AddSuccess(Resources.Recruitment.JobSearch.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void ValidateStudy(JobSearchAddModel model, Response response)
        {
            if (model.StudyRequired && string.IsNullOrWhiteSpace(model.Study))
                response.AddError(Resources.Recruitment.JobSearch.StudyRequired);
        }

        private void ValidateLanguage(JobSearchAddModel model, Response response)
        {
            if (model.LanguageRequired && string.IsNullOrWhiteSpace(model.Language))
                response.AddError(Resources.Recruitment.JobSearch.LanguageRequired);
        }

        public Response ChangeStatus(int id, JobSearchChangeStatusModel parameter)
        {
            var response = new Response();

            if (!parameter.Date.HasValue) response.AddError(Resources.Recruitment.JobSearch.DateRequired);

            if(!parameter.Status.HasValue) response.AddError(Resources.Recruitment.JobSearch.StatusRequired);

            if (response.HasErrors()) return response;

            var jobsearch = unitOfWork.JobSearchRepository.Get(id);

            if (jobsearch == null)
            {
                response.AddError(Resources.Recruitment.JobSearch.NotFound);
                return response;
            }

            if (parameter.Status == JobSearchStatus.Reopen)
            {
                if (jobsearch.Status != JobSearchStatus.Suspended)
                {
                    response.AddError(Resources.Recruitment.JobSearch.CannotChangeStatus);
                }
                else
                {
                    if (jobsearch.SuspendedDate.GetValueOrDefault().Date > parameter.Date.GetValueOrDefault())
                    {
                        response.AddError(Resources.Recruitment.JobSearch.SuspendedDateGreaterThanReopenDate);
                    }
                    else
                    {
                        jobsearch.ReopenDate = parameter.Date.GetValueOrDefault();
                    }
                }
            }

            if (parameter.Status == JobSearchStatus.Suspended)
            {
                if (jobsearch.Status != JobSearchStatus.Open && jobsearch.Status != JobSearchStatus.Reopen)
                {
                    response.AddError(Resources.Recruitment.JobSearch.CannotChangeStatus);
                }
                else
                {
                    if (jobsearch.CreatedDate.Date > parameter.Date.GetValueOrDefault())
                    {
                        response.AddError(Resources.Recruitment.JobSearch.CreatedDateGreaterThanSuspendedDate);
                    }
                    else
                    {
                        jobsearch.SuspendedDate = parameter.Date.GetValueOrDefault();
                    }
                }
            }

            if (parameter.Status == JobSearchStatus.Close)
            {
                if (jobsearch.Status == JobSearchStatus.Suspended)
                {
                    response.AddError(Resources.Recruitment.JobSearch.CannotChangeStatus);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(parameter.Reason))
                    {
                        response.AddError(Resources.Recruitment.JobSearch.ReasonCauseRequired);
                    }
                    else
                    {
                        DateTime dateToCompare = jobsearch.ReopenDate ?? jobsearch.CreatedDate;

                        if (dateToCompare.Date > parameter.Date.GetValueOrDefault())
                        {
                            response.AddError(Resources.Recruitment.JobSearch.CreatedDateGreaterThanSuspendedDate);
                        }
                        else
                        {
                            jobsearch.CloseDate = parameter.Date.GetValueOrDefault();
                            jobsearch.ReasonComments = parameter.Reason;
                        }
                    }
                }
            }

            if (response.HasErrors()) return response;

            try
            {
                jobsearch.Status = parameter.Status.GetValueOrDefault();
                unitOfWork.JobSearchRepository.Update(jobsearch);
                unitOfWork.Save();

                response.AddSuccess(Resources.Recruitment.JobSearch.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<OptionModel>> GetApplicants()
        {
            var response = new Response<IList<OptionModel>> { Data = new List<OptionModel>() };

            var leaders = unitOfWork.UserRepository.GetByGroup(emailConfig.JobSearchApplicantsCode);

            response.Data = leaders.Select(x => new OptionModel {Id = x.Id, Text = x.Name}).ToList();

            return response;
        }

        public Response<IList<OptionModel>> GetRecruiters()
        {
            var response = new Response<IList<OptionModel>> { Data = new List<OptionModel>() };

            var recruiters = unitOfWork.UserRepository.GetByGroup(emailConfig.RecruitersCode);

            response.Data = recruiters.Select(x => new OptionModel { Id = x.Id, Text = x.Name }).ToList();

            return response;
        }

        public Response<IList<JobSearchResultModel>> Search(JobSearchParameter parameter)
        {
            var response = new Response<IList<JobSearchResultModel>> { Data = new List<JobSearchResultModel>() };

            if (parameter.Status == null || !parameter.Status.Any())
            {
                parameter.Status = new List<JobSearchStatus>();

                parameter.Status.Add(JobSearchStatus.Open);
                parameter.Status.Add(JobSearchStatus.Close);
                parameter.Status.Add(JobSearchStatus.Reopen);
                parameter.Status.Add(JobSearchStatus.Suspended);
            }

            var list = unitOfWork.JobSearchRepository.Search(parameter);

            if (list.Any())
            {
                response.Data = list.Select(x => new JobSearchResultModel(x)).ToList();
            }

            return response;
        }

        public Response<JobSearchModel> Get(int id)
        {
            var response = new Response<JobSearchModel>();

            var jobsearch = unitOfWork.JobSearchRepository.GetDetail(id);

            if (jobsearch == null)
            {
                response.AddError(Resources.Recruitment.JobSearch.NotFound);
                return response;
            }

            response.Data = new JobSearchModel(jobsearch);

            return response;
        }

        private void ValidateTimeHiring(JobSearchAddModel model, Response response)
        {
            if (!model.TimeHiringId.HasValue)
            {
                response.AddError(Resources.Recruitment.JobSearch.TimeHiringRequired);
            }
        }

        private void ValidateUser(JobSearchAddModel model, Response response)
        {
            if (!model.UserId.HasValue)
            {
                response.AddError(Resources.Recruitment.JobSearch.UserRequired);
            }
            else if (!unitOfWork.UserRepository.ExistById(model.UserId.Value))
            {
                response.AddError(Resources.Admin.User.NotFound);
            }
        }

        private void ValidateMarketStudy(JobSearchAddModel model, Response response)
        {
            if (model.IsMarketStudy && string.IsNullOrWhiteSpace(model.MarketStudy))
            {
                response.AddError(Resources.Recruitment.JobSearch.MarketStudyRequired);
            }
        }

        private void ValidateSelector(JobSearchAddModel model, Response response)
        {
            if (model.RecruiterId.HasValue && !unitOfWork.UserRepository.ExistById(model.RecruiterId.Value))
            {
                response.AddError(Resources.Admin.User.NotFound);
            }
        }

        private void ValidateReasonCause(JobSearchAddModel model, Response response)
        {
            if (!model.ReasonCauseId.HasValue)
            {
                response.AddError(Resources.Recruitment.JobSearch.ReasonCauseRequired);
            }
        }

        private static void ValidateQuantity(JobSearchAddModel model, Response response)
        {
            if (!model.Quantity.HasValue)
            {
                response.AddError(Resources.Recruitment.JobSearch.QuantityRequired);
            }
        }

        private void ValidateMaximunSalary(JobSearchAddModel model, Response response)
        {
            if (!model.MaximunSalary.HasValue)
            {
                response.AddError(Resources.Recruitment.JobSearch.MaximunSalaryIsRequired);
            }
        }

        private void ValidateClient(JobSearchAddModel model, Response response)
        {
            if (!model.IsMarketStudy)
            {
                if (string.IsNullOrWhiteSpace(model.ClientCrmId))
                {
                    response.AddError(Resources.Recruitment.JobSearch.ClientRequired);
                }
                else
                {
                    var customer = unitOfWork.CustomerRepository.GetByIdCrm(model.ClientCrmId);

                    if (customer == null)
                    {
                        response.AddError(Resources.Billing.Customer.NotFound);
                    }
                    else
                    {
                        model.ClientId = customer.Id;
                    }
                }
            }
            else
            {
                model.ClientId = null;

                if (!string.IsNullOrWhiteSpace(model.ClientCrmId))
                {
                    response.AddError(Resources.Recruitment.JobSearch.ClientMustBeNull);
                }       
            }
        }
    }
}
