using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class RefundService : IRefundService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RefundService> logger;
        private readonly IRefundValidation validation;
        private readonly AppSetting settings;
        private readonly IUserData userData;

        public RefundService(IUnitOfWork unitOfWork,
            ILogMailer<RefundService> logger,
            IRefundValidation validation,
            IUserData userData,
            IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.validation = validation;
            this.settings = settingOptions.Value;
            this.userData = userData;
        }

        public Response<string> Add(RefundModel model)
        {
            var response = new Response<string>();

            validation.ValidateAdd(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain();
                domain.StatusId = settings.WorkflowStatusDraft;
                domain.AdvancementRefunds = new List<AdvancementRefund>();

                foreach (var advancement in model.Advancements)
                {
                    domain.AdvancementRefunds.Add(new AdvancementRefund { AdvancementId = advancement, Refund = domain });
                }

                unitOfWork.RefundRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Refund.AddSuccess);

                response.Data = domain.Id.ToString();
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }
    }
}
