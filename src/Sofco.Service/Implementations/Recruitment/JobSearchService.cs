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

        public Response<IList<OptionModel>> GetApplicants()
        {
            var response = new Response<IList<OptionModel>> { Data = new List<OptionModel>() };

            var leaders = unitOfWork.UserRepository.GetByGroup(emailConfig.LeadersCode);
            var recruiters = unitOfWork.UserRepository.GetByGroup(emailConfig.RecruitersCode);
            var managers = unitOfWork.UserRepository.GetByGroup(emailConfig.ManagersCode);

            response.Data = leaders.Select(x => new OptionModel {Id = x.Id, Text = x.Name}).ToList();

            foreach (var recruiter in recruiters)
            {
                if (response.Data.All(x => x.Id != recruiter.Id))
                {
                    response.Data.Add(new OptionModel { Id = recruiter.Id, Text = recruiter.Name });
                }
            }

            foreach (var manager in managers)
            {
                if (response.Data.All(x => x.Id != manager.Id))
                {
                    response.Data.Add(new OptionModel { Id = manager.Id, Text = manager.Name });
                }
            }

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



            return response;
        }

        private void ValidateTimeHiring(JobSearchAddModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.TimeHiring))
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

        private void ValidateSelector(JobSearchAddModel model, Response response)
        {
            if (!model.UserId.HasValue)
            {
                response.AddError(Resources.Recruitment.JobSearch.SelectorRequired);
            }
            else if (!unitOfWork.UserRepository.ExistById(model.UserId.Value))
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
    }
}
