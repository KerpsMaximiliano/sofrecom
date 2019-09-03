using System;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.Domain.Models.Recruitment;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Recruitment
{
    public class JobSearchService : IJobSearchService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<JobSearchService> logger;
        private readonly IUserData userData;

        public JobSearchService(IUnitOfWork unitOfWork, ILogMailer<JobSearchService> logger, IUserData userData)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
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
            if (!model.ClientId.HasValue)
            {
                response.AddError(Resources.Recruitment.JobSearch.ClientRequired);
            }
            else
            {
                var customer = unitOfWork.CustomerRepository.Get(model.ClientId.Value);
                if (customer == null)
                {
                    response.AddError(Resources.Billing.Customer.NotFound);
                }
            }
        }
    }
}
