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
using Sofco.Domain.Enums;
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
                var employeeDiccionary = new Dictionary<string, string>();

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
                            Advancements = new List<AdvancementUnrelatedItem>()
                        };

                        if (refundAux.UserApplicant != null && !employeeDiccionary.ContainsKey(refundAux.UserApplicant.Email))
                        {
                            var employee = unitOfWork.EmployeeRepository.GetByEmail(refundAux.UserApplicant?.Email);

                            employeeDiccionary.Add(refundAux.UserApplicant.Email, employee.Name);
                        }

                        if (refundAux.UserApplicant != null && employeeDiccionary.ContainsKey(refundAux.UserApplicant.Email))
                        {
                            currentAccountModel.User = employeeDiccionary[refundAux.UserApplicant.Email];
                        }
                        
                        response.Data.Add(currentAccountModel);
                    }
                }

                var advancements = unitOfWork.AdvancementRepository.GetAllApproved(settings.WorkflowStatusApproveId, AdvancementType.Viaticum);

                foreach (var advancement in advancements)
                {
                    var currentAccountModel = response.Data.FirstOrDefault(x => x.CurrencyId == advancement.CurrencyId && x.UserId == advancement.UserApplicantId);

                    if (currentAccountModel != null)
                    {
                        currentAccountModel.AdvancementTotal += advancement.Ammount;

                        currentAccountModel.Advancements.Add(new AdvancementUnrelatedItem
                        {
                            Id = advancement.Id,
                            Ammount = advancement.Ammount,
                            CurrencyId = advancement.CurrencyId,
                            CurrencyText = advancement.Currency?.Text,
                            Text = $"#{advancement.Id} - {advancement.CreationDate:dd/MM/yyyy} - {advancement.Ammount} {advancement.Currency?.Text}"
                        });
                    }
                    else
                    {
                        currentAccountModel = new CurrentAccountModel
                        {
                            User = advancement.UserApplicant?.Name,
                            UserId = advancement.UserApplicantId,
                            Currency = advancement.Currency?.Text,
                            CurrencyId = advancement.CurrencyId,
                            RefundTotal = 0,
                            AdvancementTotal = advancement.Ammount,
                            Refunds = new List<CurrentAccountRefundModel>(),
                            Advancements = new List<AdvancementUnrelatedItem>
                            {
                                new AdvancementUnrelatedItem
                                {
                                    Id = advancement.Id,
                                    Ammount = advancement.Ammount,
                                    CurrencyId = advancement.CurrencyId,
                                    CurrencyText = advancement.Currency?.Text,
                                    Text = $"#{advancement.Id} - {advancement.CreationDate:dd/MM/yyyy} - {advancement.Ammount} {advancement.Currency?.Text}"
                                }
                            }
                        };

                        if (advancement.UserApplicant != null && !employeeDiccionary.ContainsKey(advancement.UserApplicant.Email))
                        {
                            var employee = unitOfWork.EmployeeRepository.GetByEmail(advancement.UserApplicant?.Email);

                            employeeDiccionary.Add(advancement.UserApplicant.Email, employee.Name);
                        }

                        if (advancement.UserApplicant != null && employeeDiccionary.ContainsKey(advancement.UserApplicant.Email))
                        {
                            currentAccountModel.User = employeeDiccionary[advancement.UserApplicant.Email];
                        }

                        response.Data.Add(currentAccountModel);
                    }
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
