using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Validations.AdvancementAndRefund
{
    public class AdvancementValidation : IAdvancemenValidation
    {
        private readonly IUnitOfWork unitOfWork;

        public AdvancementValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Response ValidateAdd(AdvancementModel model)
        {
            var response = new Response();

            if (model == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.NullModel);
                return response;
            }

            ValidateUser(model, response);
            ValidatePaymentForm(model, response);
            ValidateType(model, response);
            ValidateAdvancementReturnForm(model, response);
            ValidateStartDateReturn(model, response);
            ValidateAnalytic(model, response);
            ValidateCurrency(model, response);
            ValidateDetails(model, response);

            return response;
        }

        private void ValidateDetails(AdvancementModel model, Response response)
        {
            if (model.Type.HasValue)
            {
                if (model.Type == AdvancementType.Salary)
                {
                    ValidateAdvancementSalary(model, response);
                }
            }
        }

        private void ValidateCurrency(AdvancementModel model, Response response)
        {
            if (!model.CurrencyId.HasValue || model.CurrencyId.Value == 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.CurrencyRequired);
            }
            else if (unitOfWork.UtilsRepository.ExistCurrency(model.CurrencyId.Value))
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.CurrencyNotFound);
            }
        }

        private void ValidateAdvancementSalary(AdvancementModel model, Response response)
        {
            if (!model.Details.Any() || model.Details.Count > 1)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.MustHaveOneDetail);
            }
            else
            {
                var item = model.Details.First();
                ValidateItem(response, item);
            }
        }

        private void ValidateItem(Response response, AdvancementDetailModel item)
        {
            if (!item.Date.HasValue)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.DateItemRequired);
            }

            if (string.IsNullOrWhiteSpace(item.Description))
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.DescriptionItemRequired);
            }

            if (!item.Ammount.HasValue || item.Ammount.Value == 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.AmmountItemRequired);
            }
        }

        private void ValidateAnalytic(AdvancementModel model, Response response)
        {
            if (!model.AnalyticId.HasValue || model.AnalyticId.Value == 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.AnalyticRequired);
            }
            else if (unitOfWork.AnalyticRepository.Exist(model.AnalyticId.Value))
            {
                response.AddError(Resources.AllocationManagement.Analytic.NotFound);
            }
        }

        private void ValidateStartDateReturn(AdvancementModel model, Response response)
        {
            if (!model.StartDateReturn.HasValue)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.StartDateReturnRequired);
            }
        }

        private void ValidateAdvancementReturnForm(AdvancementModel model, Response response)
        {
            if (!model.AdvancementReturnFormId.HasValue || model.AdvancementReturnFormId.Value == 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.AdvancementReturnFormRequired);
            }
            else if (unitOfWork.UtilsRepository.ExistAdvancementReturnForm(model.AdvancementReturnFormId.Value))
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.AdvancementReturnFormNotFound);
            }
        }

        private void ValidateType(AdvancementModel model, Response response)
        {
            if (!model.Type.HasValue)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.TypeRequired);
            }
        }

        private void ValidatePaymentForm(AdvancementModel model, Response response)
        {
            if (!model.PaymentForm.HasValue)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.PaymentFormRequired);
            }
        }

        private void ValidateUser(AdvancementModel model, Response response)
        {
            if (!model.UserApplicantId.HasValue || model.UserApplicantId.Value == 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.UserApplicantRequired);
            }
            else if (!unitOfWork.UserRepository.ExistById(model.UserApplicantId.Value))
            {
                response.AddError(Resources.Admin.User.NotFound);
            }
        }
    }
}
