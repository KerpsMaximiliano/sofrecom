using System;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Validations.AdvancementAndRefund
{
    public class RefundValidation: IRefundValidation
    {
        private readonly IUnitOfWork unitOfWork;

        public RefundValidation(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void ValidateAdd(RefundModel model, Response response)
        {
            if (model == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.NullModel);
                return;
            }

            ValidateCommonData(model, response);
            ValidateDetails(model, response);
            ValidateAdvancements(model, response);
        }

        public Response ValidateUpdate(RefundModel model)
        {
            var response = new Response();

            if (model == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.NullModel);
                return response;
            }

            ValidateIfExist(model, response);

            if (response.HasErrors()) return response;

            ValidateCommonData(model, response);
            ValidateDetails(model, response);
            ValidateAdvancements(model, response);

            return response;
        }

        public bool HasUserRefund(Refund refund)
        {
            var diffTotal = refund.AdvancementRefunds
                                .Sum(x => x.OriginalAdvancement) - refund.Details.Sum(x => x.Ammount);

            var diff = diffTotal < 0
                ? Math.Abs(diffTotal)
                : 0;

            return diff > 0;
        }

        private void ValidateIfExist(RefundModel model, Response response)
        {
            if (!unitOfWork.RefundRepository.Exist(model.Id))
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.NotFound);
            }
        }

        private void ValidateAdvancements(RefundModel model, Response response)
        {
            if (model.Advancements == null) return;

            foreach (var advancement in model.Advancements)
            {
                if (!unitOfWork.AdvancementRepository.Exist(advancement))
                {
                    response.AddError(Resources.AdvancementAndRefund.Advancement.NotFound);
                }
            }
        }

        private void ValidateDetails(RefundModel model, Response response)
        {
            if (!model.Details.Any())
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.DetailsRequired);
            }
            else
            {
                foreach (var detail in model.Details)
                {
                    ValidateDate(detail, response);
                    ValidateAmmount(detail, response);
                    ValidateDescription(detail, response);
                }
            }
        }

        private void ValidateDescription(RefundDetailModel detail, Response response)
        {
            if (string.IsNullOrWhiteSpace(detail.Description))
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.DetailDescriptionRequired);
            }
        }

        private void ValidateAmmount(RefundDetailModel detail, Response response)
        {
            if (detail.Ammount <= 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.DetailAmmountRequired);
            }
        }

        private void ValidateDate(RefundDetailModel detail, Response response)
        {
            if (!detail.CreationDate.HasValue)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.DetailDateRequired);
                return;
            }

            if (detail.CreationDate.Value.ToUniversalTime() > DateTime.UtcNow)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.DetailDateInvalidRange);
            }
        }

        private void ValidateCommonData(RefundModel model, Response response)
        {
            ValidateUser(model, response);
            ValidateCurrency(model, response);
            ValidateAnalytic(model, response);
            ValidateCreditCard(model, response);
        }

        private void ValidateCreditCard(RefundModel model, Response response)
        {
            if (model.HasCreditCard)
            {
                if (!model.CreditCardId.HasValue || model.CreditCardId.Value <= 0)
                {
                    response.AddError(Resources.AdvancementAndRefund.Refund.CreditCardRequired);
                }
                else if (!unitOfWork.UtilsRepository.ExistCreditCard(model.CreditCardId.Value))
                {
                    response.AddError(Resources.AdvancementAndRefund.Refund.CreditCardNotFound);
                }
            }
        }

        private void ValidateAnalytic(RefundModel model, Response response)
        {
            if (!model.AnalyticId.HasValue || model.AnalyticId.Value <= 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.AnalyticRequired);
            }
            else if (!unitOfWork.AnalyticRepository.Exist(model.AnalyticId.Value))
            {
                response.AddError(Resources.AllocationManagement.Analytic.NotFound);
            }
        }

        private void ValidateCurrency(RefundModel model, Response response)
        {
            if (!model.CurrencyId.HasValue || model.CurrencyId.Value <= 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.CurrencyRequired);
            }
            else if (!unitOfWork.UtilsRepository.ExistCurrency(model.CurrencyId.Value))
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.CurrencyNotFound);
            }
        }

        private void ValidateUser(RefundModel model, Response response)
        {
            if (!model.UserApplicantId.HasValue || model.UserApplicantId.Value <= 0)
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
