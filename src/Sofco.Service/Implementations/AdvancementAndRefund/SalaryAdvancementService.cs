using System;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Utils;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class SalaryAdvancementService : ISalaryAdvancementService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<SalaryAdvancementService> logger;
        private readonly IAdvancemenValidation validation;
        private readonly AppSetting settings;

        public SalaryAdvancementService(IUnitOfWork unitOfWork,
            ILogMailer<SalaryAdvancementService> logger,
            IAdvancemenValidation validation,
            IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.validation = validation;
            this.settings = settingOptions.Value;
        }

        public Response<IList<SalaryAdvancementModel>> Get()
        {
            var response = new Response<IList<SalaryAdvancementModel>> { Data = new List<SalaryAdvancementModel>() };

            var advancements = unitOfWork.AdvancementRepository.GetSalaryResume(settings.SalaryWorkflowId, settings.WorkflowStatusApproveId);

            foreach (var advancement in advancements)
            {
                var modelExist = response.Data.SingleOrDefault(x => x.UserId == advancement.UserApplicantId);

                if (modelExist == null)
                {
                    var model = new SalaryAdvancementModel();

                    model.UserId = advancement.UserApplicantId;
                    model.UserName = advancement.UserApplicant.Name;
                    model.TotalAmount = advancement.Ammount;
                    model.DiscountedAmount = advancement.Discounts.Sum(x => x.Amount);
                    model.Advancements = new List<SalaryAdvancementModelItem>();

                    model.Advancements.Add(new SalaryAdvancementModelItem
                    {
                        AdvancementId = advancement.Id,
                        ReturnForm = advancement.AdvancementReturnForm,
                        Total = advancement.Ammount,
                        Discounts = advancement.Discounts.Select(s => new SalaryDiscountModel
                        {
                            Date = s.Date,
                            Id = s.Id,
                            Amount = s.Amount
                        }).ToList()

                    });

                    response.Data.Add(model);
                }
                else
                {
                    modelExist.TotalAmount += advancement.Ammount;
                    modelExist.DiscountedAmount += advancement.Discounts.Sum(x => x.Amount);

                    if(modelExist.Advancements == null) modelExist.Advancements = new List<SalaryAdvancementModelItem>();

                    modelExist.Advancements.Add(new SalaryAdvancementModelItem
                    {
                        AdvancementId = advancement.Id,
                        ReturnForm = advancement.AdvancementReturnForm,
                        Total = advancement.Ammount,
                        Discounts = advancement.Discounts.Select(s => new SalaryDiscountModel
                        {
                            Date = s.Date,
                            Id = s.Id,
                            Amount = s.Amount
                        }).ToList()

                    });
                }
            }

            return response;
        }

        public Response<SalaryDiscountModel> Add(SalaryDiscountAddModel model)
        {
            var response = new Response<SalaryDiscountModel>();

            if(!model.Date.HasValue)
                response.AddError(Resources.AdvancementAndRefund.Advancement.SalaryDateRequired);

            if(!model.Amount.HasValue || model.Amount < 0)
                response.AddError(Resources.AdvancementAndRefund.Advancement.SalaryAmountRequired);

            var advancement = unitOfWork.AdvancementRepository.GetWithDiscounts(model.AdvancementId);

            if(advancement == null)
                response.AddError(Resources.AdvancementAndRefund.Advancement.NotFound);

            if (response.HasErrors()) return response;

            var discount = model.Amount.GetValueOrDefault();

            if (advancement.Discounts != null && advancement.Discounts.Any())
            {
                discount += advancement.Discounts.Sum(x => x.Amount);
            }

            if (advancement.Ammount < discount)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.SalaryAmountGreaterThanTotalAdvancement);
                return response;
            }

            try
            {
                var domain = new SalaryDiscount
                {
                    Date = model.Date.GetValueOrDefault().Date,
                    Amount = model.Amount.GetValueOrDefault(),
                    AdvancementId = model.AdvancementId
                };

                unitOfWork.AdvancementRepository.AddSalaryDiscount(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Advancement.SalaryAdvancementAdded);

                response.Data = new SalaryDiscountModel
                {
                    Id = domain.Id,
                    Date = domain.Date,
                    Amount = domain.Amount
                };
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var salaryDiscount = unitOfWork.AdvancementRepository.GetSalaryDiscount(id);

            if (salaryDiscount == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.SalaryDiscountNotFound);
                return response;
            }

            try
            {
                unitOfWork.AdvancementRepository.DeleteSalaryDiscount(salaryDiscount);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Advancement.SalaryAdvancementDeleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }
    }
}
