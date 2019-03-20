using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Services.AdvancementAndRefund;
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

                var userDiccionary = new Dictionary<int, List<Refund>>();

                foreach (var refund in refunds)
                {
                    if (userDiccionary.ContainsKey(refund.UserApplicantId))
                        userDiccionary[refund.UserApplicantId].Add(refund);
                    else
                        userDiccionary.Add(refund.UserApplicantId, new List<Refund> { refund });
                }

                foreach (var key in userDiccionary.Keys)
                {
                    var userRefunds = userDiccionary[key];

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
                        var allRefundAux = refunds.Where(x => x.CurrencyId == currencyDiccionaryKey && x.UserApplicantId == key).ToList();
                        var refundAux = allRefundAux.FirstOrDefault();

                        if (refundAux == null) continue;

                        var currentAccountModel = new CurrentAccountModel
                        {
                            User = refundAux.UserApplicant?.Name,
                            UserId = refundAux.UserApplicantId,
                            Currency = refundAux.Currency?.Text,
                            CurrencyId = currencyDiccionaryKey,
                            RefundTotal = currencyDiccionary[currencyDiccionaryKey],
                            Refunds = allRefundAux.Select(x =>
                                new CurrentAccountRefundModel()
                                {
                                    Id = x.Id,
                                    Value = x.TotalAmmount,
                                    Advancements = string.Join(" - ", x.AdvancementRefunds.Select(a => $"#{a.AdvancementId}")),
                                }).ToList(),
                        };

                        response.Data.Add(currentAccountModel);
                    }
                }

                var advancements = unitOfWork.AdvancementRepository.GetAllApproved(settings.WorkflowStatusApproveId);

                foreach (var currentAccountModel in response.Data)
                {
                    var advancementsAux = advancements.Where(x => x.CurrencyId == currentAccountModel.CurrencyId && x.UserApplicantId == currentAccountModel.UserId).ToList();

                    currentAccountModel.AdvancementTotal = advancementsAux.Sum(x => x.Ammount);

                    currentAccountModel.Advancements = advancementsAux.Select(x => new AdvancementUnrelatedItem
                    {
                        Id = x.Id,
                        Ammount = x.Ammount,
                        CurrencyId = x.CurrencyId,
                        CurrencyText = x.Currency?.Text,
                        Text = $"#{x.Id} - {x.CreationDate:dd/MM/yyyy} - {x.Ammount} {x.Currency?.Text}"
                    }).ToList();

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

        public Response UpdateMassive(UpdateMassiveModel model)
        {
            var response = new Response();

            if (!model.Advancements.Any() || !model.Refunds.Any())
            {
                response.AddError(Resources.AdvancementAndRefund.Setting.ModelEmpty);
                return response;
            }

            try
            {
                foreach (var advancement in model.Advancements)
                {
                    if (!unitOfWork.AdvancementRepository.Exist(advancement)) continue;

                    foreach (var refund in model.Refunds)
                    {
                        if (!unitOfWork.RefundRepository.Exist(refund)) continue;

                        if (!unitOfWork.RefundRepository.ExistAdvancementRefund(advancement, refund))
                        {
                            unitOfWork.RefundRepository.AddAdvancementRefund(new AdvancementRefund { AdvancementId = advancement, RefundId = refund });
                        }
                    }
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.AdvancementAndRefund.Setting.UpdateMassiveSuccess);
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
