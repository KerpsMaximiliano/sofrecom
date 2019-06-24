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

                    //SalaryAdvancementModelItem
                    //SalaryDiscountModel

                    model.Advancements = advancement.Discounts.Select(x => new SalaryAdvancementModelItem
                    {
                        AdvancementId = advancement.Id,
                        ReturnForm = advancement.AdvancementReturnForm,
                        Discounts = advancement.Discounts.Select(s => new SalaryDiscountModel
                        {
                            Date = s.Date,
                            Amount = s.Amount
                        }).ToList()

                    }).ToList();

                    response.Data.Add(model);
                }
                else
                {
                    modelExist.TotalAmount += advancement.Ammount;
                    modelExist.DiscountedAmount += advancement.Discounts.Sum(x => x.Amount);

                    modelExist.Advancements = advancement.Discounts.Select(x => new SalaryAdvancementModelItem
                    {
                        AdvancementId = advancement.Id,
                        ReturnForm = advancement.AdvancementReturnForm,
                        Discounts = advancement.Discounts.Select(s => new SalaryDiscountModel
                        {
                            Date = s.Date,
                            Amount = s.Amount
                        }).ToList()

                    }).ToList();
                }
            }

            return response;
        }
    }
}
