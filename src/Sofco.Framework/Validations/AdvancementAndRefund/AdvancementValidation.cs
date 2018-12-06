﻿using System;
using Sofco.Core.DAL;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
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

        public void ValidateAdd(AdvancementModel model, Response response)
        {
            if (model == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.NullModel);
                return;
            }

            ValidateCommonData(model, response);
        }

        public Response ValidateUpdate(AdvancementModel model)
        {
            var response = new Response();

            if (model == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.NullModel);
                return response;
            }

            ValidateIfExist(model, response);

            if (response.HasErrors()) return response;

            ValidateCommonData(model, response);

            return response;
        }

        private void ValidateIfExist(AdvancementModel model, Response response)
        {
            if (!unitOfWork.AdvancementRepository.Exist(model.Id))
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.NotFound);
            }
        }

        private void ValidateCommonData(AdvancementModel model, Response response)
        {
            ValidateUser(model, response);
            ValidatePaymentForm(model, response);
            ValidateType(model, response);
            ValidateAdvancementReturnForm(model, response);
            ValidateStartDateReturn(model, response);
            ValidateCurrency(model, response);
            ValidateDescription(model, response);
            ValidateAmmount(model, response);
        }

        private void ValidateCurrency(AdvancementModel model, Response response)
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

        private void ValidateDescription(AdvancementModel model, Response response)
        {
            if (string.IsNullOrWhiteSpace(model.Description))
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.DescriptionItemRequired);
            }
        }

        private void ValidateAmmount(AdvancementModel model, Response response)
        {
            if (!model.Ammount.HasValue || model.Ammount.Value <= 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.AmmountItemRequired);
            }
        }

        private void ValidateStartDateReturn(AdvancementModel model, Response response)
        {
            if (!model.StartDateReturn.HasValue)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.StartDateReturnRequired);
            }
            else if (model.StartDateReturn.Value.Date < DateTime.Today.Date)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.StartDateReturnLessThanToday);
            }
        }

        private void ValidateAdvancementReturnForm(AdvancementModel model, Response response)
        {
            if (!model.AdvancementReturnFormId.HasValue || model.AdvancementReturnFormId.Value <= 0)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.AdvancementReturnFormRequired);
            }
            else if (!unitOfWork.UtilsRepository.ExistAdvancementReturnForm(model.AdvancementReturnFormId.Value))
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
