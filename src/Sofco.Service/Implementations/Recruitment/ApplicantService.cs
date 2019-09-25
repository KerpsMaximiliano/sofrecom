using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Service.Implementations.Recruitment
{
    public class ApplicantService : IApplicantService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ApplicantService> logger;
        private readonly IUserData userData;

        public ApplicantService(IUnitOfWork unitOfWork, ILogMailer<ApplicantService> logger, IUserData userData)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public Response Add(ApplicantAddModel model)
        {
            var response = new Response();

            if (model == null)
            {
                response.AddError(Resources.Recruitment.Applicant.ModelNull);
                return response;
            }

            ValidateFirstName(model, response);
            ValidateLastName(model, response);
            ValidateCommets(model, response);
            ValidateEmail(model, response);
            ValidateTelephone1(model, response);
            ValidateTelephone2(model, response);
            ValidateClient(model, response);
            ValidateRecommendedUser(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain();
                domain.CreatedDate = DateTime.UtcNow.Date;
                domain.CreatedBy = userData.GetCurrentUser().UserName;

                unitOfWork.ApplicantRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Recruitment.Applicant.AddSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<ApplicantResultModel>> Search(ApplicantSearchParameters parameter)
        {
            var response = new Response<IList<ApplicantResultModel>> { Data = new List<ApplicantResultModel>() };

            var list = unitOfWork.ApplicantRepository.Search(parameter);

            if (list.Any())
            {
                response.Data = list.Select(x => new ApplicantResultModel(x)).ToList();
            }

            return response;
        }

        public Response Update(int id, ApplicantAddModel model)
        {
            var response = new Response();

            if (model == null)
            {
                response.AddError(Resources.Recruitment.Applicant.ModelNull);
                return response;
            }

            var applicant = unitOfWork.ApplicantRepository.GetDetail(id);

            if (applicant == null)
            {
                response.AddError(Resources.Recruitment.Applicant.NotFound);
                return response;
            }

            ValidateFirstName(model, response);
            ValidateLastName(model, response);
            ValidateCommets(model, response);
            ValidateEmail(model, response);
            ValidateTelephone1(model, response);
            ValidateTelephone2(model, response);
            ValidateClient(model, response);
            ValidateRecommendedUser(model, response);

            if (response.HasErrors()) return response;

            try
            {
                model.UpdateDomain(applicant);

                unitOfWork.ApplicantRepository.Update(applicant);
                unitOfWork.Save();

                response.AddSuccess(Resources.Recruitment.Applicant.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<ApplicantDetailModel> Get(int id)
        {
            var response = new Response<ApplicantDetailModel>();

            var applicant = unitOfWork.ApplicantRepository.GetDetail(id);

            if (applicant == null)
            {
                response.AddError(Resources.Recruitment.Applicant.NotFound);
                return response;
            }

            response.Data = new ApplicantDetailModel(applicant);

            return response;
        }

        private void ValidateRecommendedUser(ApplicantAddModel model, Response response)
        {
            if (model.RecommendedByUserId.HasValue && !unitOfWork.UserRepository.ExistById(model.RecommendedByUserId.Value))
            {
                response.AddError(Resources.Admin.User.NotFound);
            }
        }

        private void ValidateClient(ApplicantAddModel model, Response response)
        {
            if (!string.IsNullOrWhiteSpace(model.ClientCrmId))
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

        private void ValidateTelephone2(ApplicantAddModel model, Response response)
        {
            if (!string.IsNullOrWhiteSpace(model.CountryCode2) && model.CountryCode2.Length > 2)
                response.AddError(Resources.Recruitment.Applicant.CountryCode2MaxLengthError);
            if (!string.IsNullOrWhiteSpace(model.AreaCode2) && model.AreaCode2.Length > 3)
                response.AddError(Resources.Recruitment.Applicant.AreaCode2MaxLengthError);
            if (!string.IsNullOrWhiteSpace(model.Telephone2) && model.Telephone2.Length > 10)
                response.AddError(Resources.Recruitment.Applicant.Telephone2MaxLengthError);
        }

        private static void ValidateTelephone1(ApplicantAddModel model, Response response)
        {
            if (!string.IsNullOrWhiteSpace(model.CountryCode1) && model.CountryCode1.Length > 2)
                response.AddError(Resources.Recruitment.Applicant.CountryCode1MaxLengthError);
            if (!string.IsNullOrWhiteSpace(model.AreaCode1) && model.AreaCode1.Length > 3)
                response.AddError(Resources.Recruitment.Applicant.AreaCode1MaxLengthError);
            if (!string.IsNullOrWhiteSpace(model.Telephone1) && model.Telephone1.Length > 10)
                response.AddError(Resources.Recruitment.Applicant.Telephone1MaxLengthError);
        }

        private void ValidateEmail(ApplicantAddModel model, Response response)
        {
            if (!string.IsNullOrWhiteSpace(model.Email) && model.Email.Length > 75)
                response.AddError(Resources.Recruitment.Applicant.EmailMaxLengthError);
        }

        private void ValidateCommets(ApplicantAddModel model, Response response)
        {
            if (!string.IsNullOrWhiteSpace(model.Comments) && model.Comments.Length > 3000)
                response.AddError(Resources.Recruitment.Applicant.CommentsMaxLengthError);
        }

        private void ValidateLastName(ApplicantAddModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                response.AddError(Resources.Recruitment.Applicant.LastNameRequired);
            }
            else if (model.LastName.Length > 75)
            {
                response.AddError(Resources.Recruitment.Applicant.LastNameMaxLengthError);
            }
        }

        private void ValidateFirstName(ApplicantAddModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName))
            {
                response.AddError(Resources.Recruitment.Applicant.FirstNameRequired);
            }
            else if (model.FirstName.Length > 75)
            {
                response.AddError(Resources.Recruitment.Applicant.FirstNameMaxLengthError);
            }
        }
    }
}
