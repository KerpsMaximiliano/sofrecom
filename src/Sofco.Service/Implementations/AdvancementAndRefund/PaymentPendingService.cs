using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class PaymentPendingService : IPaymentPendingService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSetting settings;
        private readonly IRoleManager roleManager;

        public PaymentPendingService(IUnitOfWork unitOfWork,
            IRoleManager roleManager,
            IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.roleManager = roleManager;
            this.settings = settingOptions.Value;
        }

        public Response<IList<PaymentPendingModel>> GetAllPaymentPending()
        {
            var response = new Response<IList<PaymentPendingModel>>();
            response.Data = new List<PaymentPendingModel>();

            var employeeDicc = new Dictionary<string, string>();
            var employeeManagerDicc = new Dictionary<string, string>();
            var employeeNameManagerDicc = new Dictionary<string, string>();

            var hasAccess = roleManager.IsDafOrGaf();

            if (!hasAccess) return response;

            var currencyPesos = unitOfWork.UtilsRepository.GetCurrencies().FirstOrDefault(x => x.Id == settings.CurrencyPesos);

            FillRefunds(response, employeeDicc, employeeManagerDicc, employeeNameManagerDicc, currencyPesos);

            FillAdvancements(employeeDicc, employeeManagerDicc, employeeNameManagerDicc, response, currencyPesos);

            foreach (var row in response.Data)
            {
                var firstEntity = row.Entities.FirstOrDefault();

                if (row.Entities.All(x => x.CurrencyName == firstEntity?.CurrencyName))
                {
                    row.CanPayAll = true;
                }
            }

            return response;
        }

        private void FillRefunds(Response<IList<PaymentPendingModel>> response, Dictionary<string, string> employeeDicc,
            Dictionary<string, string> employeeManagerDicc, Dictionary<string, string> employeeNameManagerDicc,
            Currency currencyPesos)
        {
            var refunds = unitOfWork.RefundRepository.GetAllPaymentPending(settings.WorkFlowStateAccounted);

            foreach (var refund in refunds)
            {
                var itemAlreadyInList = response.Data.SingleOrDefault(x => x.UserApplicantId == refund.UserApplicantId && x.CurrencyId == refund.CurrencyId);

                if (itemAlreadyInList == null)
                {
                    var item = new PaymentPendingModel
                    {
                        Id = response.Data.Count + 1,
                        UserApplicantId = refund.UserApplicantId,
                        UserApplicantDesc = refund.UserApplicant?.Name,
                        CurrencyId = refund.CurrencyId,
                        CurrencyDesc = refund.Currency?.Text,
                        IsCurrencyPesos = currencyPesos.Id == refund.CurrencyId,
                        CurrencyExchange = refund.CurrencyExchange > 0 ? refund.CurrencyExchange : (decimal?) null,
                        Ammount = refund.TotalAmmount,
                        Entities = new List<EntityToPay> { new EntityToPay
                        {
                            Id = refund.Id,
                            Type = "refund",
                            WorkflowId = refund.WorkflowId,
                            CurrencyName = refund.CurrencyExchange > 0 ? currencyPesos.Text : refund.Currency?.Text,
                            EntitiesRelatedDesc = string.Join(" - ", refund.AdvancementRefunds.Select(a => $"#{a.AdvancementId}")),
                            EntitiesRelatedIds = refund.AdvancementRefunds.Select(a => a.AdvancementId),
                            Ammount = refund.CurrencyExchange > 0 ? refund.TotalAmmount * refund.CurrencyExchange : refund.TotalAmmount,
                            NextWorkflowStateId = settings.WorkflowStatusApproveId
                        } }
                    };

                    SetBankAndManager(refund.UserApplicant?.Email, employeeDicc, employeeManagerDicc, employeeNameManagerDicc);

                    if (refund.UserApplicant != null)
                    {
                        item.Bank = employeeDicc[refund.UserApplicant.Email];
                        item.Manager = employeeManagerDicc[refund.UserApplicant?.Email];
                        item.UserApplicantDesc = employeeNameManagerDicc[refund.UserApplicant?.Email];
                    }

                    response.Data.Add(item);
                }
                else
                {
                    itemAlreadyInList.Entities.Add(new EntityToPay
                    {
                        Id = refund.Id,
                        Type = "refund",
                        WorkflowId = refund.WorkflowId,
                        CurrencyName = refund.CurrencyExchange > 0 ? currencyPesos.Text : refund.Currency?.Text,
                        EntitiesRelatedDesc = string.Join(" - ", refund.AdvancementRefunds.Select(a => $"#{a.AdvancementId}")),
                        EntitiesRelatedIds = refund.AdvancementRefunds.Select(a => a.AdvancementId),
                        Ammount = refund.CurrencyExchange > 0 ? refund.TotalAmmount * refund.CurrencyExchange : refund.TotalAmmount,
                        NextWorkflowStateId = settings.WorkflowStatusFinalizedId,
                    });

                    itemAlreadyInList.Ammount += refund.TotalAmmount;

                    if (refund.CurrencyExchange > 0)
                    {
                        if (!itemAlreadyInList.CurrencyExchange.HasValue)
                        {
                            itemAlreadyInList.CurrencyExchange = refund.CurrencyExchange;
                        }
                        else if(itemAlreadyInList.CurrencyExchange < refund.CurrencyExchange)
                        {
                            itemAlreadyInList.CurrencyExchange = refund.CurrencyExchange;
                        }
                    }
                }
            }
        }

        private void FillAdvancements(Dictionary<string, string> employeeDicc,
            Dictionary<string, string> employeeManagerDicc, Dictionary<string, string> employeeNameManagerDicc,
            Response<IList<PaymentPendingModel>> response, Currency currencyPesos)
        {
            var advancementsAccounted = unitOfWork.AdvancementRepository.GetAllPaymentPending(settings.WorkFlowStateAccounted);
            var advancementsPaid = unitOfWork.AdvancementRepository.GetAllPaymentPending(settings.WorkflowStatusApproveId);
            

            foreach (var advancement in advancementsPaid)
            {
                if(advancement.AdvancementRefunds == null || !advancement.AdvancementRefunds.Any()) continue;

                if(advancement.AdvancementRefunds.All(x => x.Refund.InWorkflowProcess)) continue;

                var itemAlreadyInList = response.Data.SingleOrDefault(x => x.UserApplicantId == advancement.UserApplicantId && x.CurrencyId == advancement.CurrencyId);

                if (itemAlreadyInList == null)
                {
                    var ammount = advancement.Ammount;
                    var currencyName = advancement.Currency?.Text;

                    if (advancement.CurrencyId != settings.CurrencyPesos)
                    {
                        var maxExchange = advancement.AdvancementRefunds.Max(x => x.Refund.CurrencyExchange);

                        if (maxExchange > 0)
                        {
                            ammount *= maxExchange;
                            currencyName = currencyPesos?.Text;
                        }
                    }

                    var item = new PaymentPendingModel
                    {
                        Id = response.Data.Count + 1,
                        UserApplicantId = advancement.UserApplicantId,
                        UserApplicantDesc = advancement.UserApplicant?.Name,
                        CurrencyId = advancement.CurrencyId,
                        CurrencyDesc = advancement.Currency?.Text,
                        Ammount = advancement.Ammount * -1,
                        IsCurrencyPesos = currencyPesos?.Id == advancement.CurrencyId,
                        Entities = new List<EntityToPay> { new EntityToPay
                        {
                            Id = advancement.Id,
                            Type = "advancement",
                            EntityType = advancement.Type.ToString(),
                            WorkflowId = advancement.WorkflowId,
                            Ammount = ammount,
                            HasRefundsInProcess = advancement.AdvancementRefunds.Any(x => x.Refund.InWorkflowProcess),
                            CurrencyName = currencyName,
                            EntitiesRelatedDesc = string.Join(" - ", advancement.AdvancementRefunds.Select(a => $"#{a.RefundId}")),
                            EntitiesRelatedIds = advancement.AdvancementRefunds.Select(a => a.RefundId),
                            NextWorkflowStateId = settings.WorkflowStatusFinalizedId
                        } }
                    };

                    SetBankAndManager(advancement.UserApplicant?.Email, employeeDicc, employeeManagerDicc, employeeNameManagerDicc);

                    if (advancement.UserApplicant != null)
                    {
                        item.Bank = employeeDicc[advancement.UserApplicant.Email];
                        item.Manager = employeeManagerDicc[advancement.UserApplicant?.Email];
                        item.UserApplicantDesc = employeeNameManagerDicc[advancement.UserApplicant?.Email];
                    }

                    response.Data.Add(item);
                }
                else
                {
                    var entityToAdd = new EntityToPay
                    {
                        Id = advancement.Id,
                        Type = "advancement",
                        WorkflowId = advancement.WorkflowId,
                        CurrencyName = advancement.Currency.Text,
                        Ammount = advancement.Ammount,
                        HasRefundsInProcess = advancement.AdvancementRefunds.Any(x => x.Refund.InWorkflowProcess),
                        EntityType = advancement.Type.ToString(),
                        EntitiesRelatedDesc = string.Join(" - ", advancement.AdvancementRefunds.Select(a => $"#{a.RefundId}")),
                        EntitiesRelatedIds = advancement.AdvancementRefunds.Select(a => a.RefundId),
                        NextWorkflowStateId = settings.WorkflowStatusFinalizedId
                    };

                    if (advancement.CurrencyId != settings.CurrencyPesos)
                    {
                        var maxExchange = advancement.AdvancementRefunds.Max(x => x.Refund.CurrencyExchange);

                        if (maxExchange > 0)
                        {
                            entityToAdd.Ammount *= maxExchange;
                            entityToAdd.CurrencyName = currencyPesos?.Text;
                        }
                    }

                    itemAlreadyInList.Entities.Add(entityToAdd);

                    itemAlreadyInList.Ammount -= advancement.Ammount;
                }
            }

            foreach (var advancement in advancementsAccounted)
            {
                var itemAlreadyInList = response.Data.SingleOrDefault(x => x.UserApplicantId == advancement.UserApplicantId && x.CurrencyId == advancement.CurrencyId);

                if (itemAlreadyInList == null)
                {
                    var item = new PaymentPendingModel
                    {
                        Id = response.Data.Count + 1,
                        UserApplicantId = advancement.UserApplicantId,
                        UserApplicantDesc = advancement.UserApplicant?.Name,
                        CurrencyId = advancement.CurrencyId,
                        CurrencyDesc = advancement.Currency?.Text,
                        Ammount = advancement.Ammount,
                        IsCurrencyPesos = currencyPesos?.Id == advancement.CurrencyId,
                        Entities = new List<EntityToPay> { new EntityToPay
                        {
                            Id = advancement.Id,
                            Type = "advancement-accounted",
                            WorkflowId = advancement.WorkflowId,
                            EntityType = advancement.Type.ToString(),
                            Ammount = advancement.Ammount,
                            CurrencyName = advancement.Currency?.Text,
                            NextWorkflowStateId = settings.WorkflowStatusApproveId
                        } }
                    };

                    SetBankAndManager(advancement.UserApplicant?.Email, employeeDicc, employeeManagerDicc, employeeNameManagerDicc);

                    if (advancement.UserApplicant != null)
                    {
                        item.Bank = employeeDicc[advancement.UserApplicant.Email];
                        item.Manager = employeeManagerDicc[advancement.UserApplicant?.Email];
                        item.UserApplicantDesc = employeeNameManagerDicc[advancement.UserApplicant?.Email];
                    }

                    response.Data.Add(item);
                }
                else
                {
                    itemAlreadyInList.Entities.Add(new EntityToPay
                    {
                        Id = advancement.Id,
                        Type = "advancement-accounted",
                        WorkflowId = advancement.WorkflowId,
                        Ammount = advancement.Ammount,
                        CurrencyName = advancement.Currency?.Text,
                        EntityType = advancement.Type.ToString(),
                        NextWorkflowStateId = settings.WorkflowStatusApproveId
                    });

                    itemAlreadyInList.Ammount += advancement.Ammount;
                }
            }
        }

        private void SetBankAndManager(string email, Dictionary<string, string> employeeDictionary,
            Dictionary<string, string> employeeManagerDictionary, Dictionary<string, string> employeeNameDictionary)
        {
            if (!employeeDictionary.ContainsKey(email))
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmail(email);

                if (employee != null)
                {
                    employeeDictionary.Add(email, employee.Bank);
                    employeeNameDictionary.Add(email, employee.Name);
                    employeeManagerDictionary.Add(email, employee.Manager?.Name);
                }
            }
        }
    }
}