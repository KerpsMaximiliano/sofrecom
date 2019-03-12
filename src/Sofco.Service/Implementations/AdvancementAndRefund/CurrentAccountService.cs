using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class CurrentAccountService : ICurrentAccountService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CurrentAccountService> logger;
        private readonly AppSetting settings;

        public CurrentAccountService(IUnitOfWork unitOfWork,
            IOptions<AppSetting> settingsOptions,
            ILogMailer<CurrentAccountService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.settings = settingsOptions.Value;
        }

        public Response<IList<CurrentAccountModel>> Get()
        {
            var response = new Response<IList<CurrentAccountModel>> { Data = new List<CurrentAccountModel>() };

            try
            {
                var refunds = unitOfWork.RefundRepository.GetAllInCurrentAccount(settings.WorkflowStatusCurrentAccount);

                var diccionary = new Dictionary<int, List<Refund>>();

                foreach (var refund in refunds)
                {
                    if (diccionary.ContainsKey(refund.UserApplicantId))
                        diccionary[refund.UserApplicantId].Add(refund);
                    else
                        diccionary.Add(refund.UserApplicantId, new List<Refund> { refund });
                }

                foreach (var key in diccionary.Keys)
                {
                    var userRefunds = diccionary[key];

                    var currencyDiccionary = new Dictionary<int, decimal>();

                    foreach (var userRefund in userRefunds)
                    {
                        if (currencyDiccionary.ContainsKey(userRefund.CurrencyId))
                            currencyDiccionary[userRefund.CurrencyId] += userRefund.TotalAmmount;
                        else
                            currencyDiccionary.Add(userRefund.CurrencyId, userRefund.TotalAmmount);
                    }

                    foreach (var currencyDiccionaryKey in currencyDiccionary.Keys)
                    {
                        var refundAux = refunds.FirstOrDefault(x => x.CurrencyId == currencyDiccionaryKey && x.UserApplicantId == key);

                        if (refundAux == null) continue;

                        var currentAccountModel = new CurrentAccountModel
                        {
                            User = refundAux.UserApplicant?.Name,
                            UserId = refundAux.UserApplicantId,
                            Currency = refundAux.Currency?.Text,
                            CurrencyId = currencyDiccionaryKey,
                            RefundTotal = currencyDiccionary[currencyDiccionaryKey]
                        };

                        response.Data.Add(currentAccountModel);
                    }
                }

                var advancements = unitOfWork.AdvancementRepository.GetAllApproved(settings.WorkflowStatusApproveId);

                foreach (var currentAccountModel in response.Data)
                {
                    currentAccountModel.AdvancementTotal = advancements.Where(x => x.CurrencyId == currentAccountModel.CurrencyId && x.UserApplicantId == currentAccountModel.UserId).Sum(x => x.Ammount);

                    if (currentAccountModel.RefundTotal > currentAccountModel.AdvancementTotal)
                        currentAccountModel.UserRefund = currentAccountModel.RefundTotal - currentAccountModel.AdvancementTotal;
                    else
                        currentAccountModel.CompanyRefund = currentAccountModel.AdvancementTotal - currentAccountModel.RefundTotal;
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }
    }
}
