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

            FillAdvancements(employeeDicc, employeeManagerDicc, employeeNameManagerDicc, response);

            FillRefunds(response, employeeDicc, employeeManagerDicc, employeeNameManagerDicc);

            return response;
        }

        private void FillRefunds(Response<IList<PaymentPendingModel>> response, Dictionary<string, string> employeeDicc, Dictionary<string, string> employeeManagerDicc, Dictionary<string, string> employeeNameManagerDicc)
        {
            var refunds = unitOfWork.RefundRepository.GetAllPaymentPending(settings.WorkFlowStateAccounted);

            foreach (var refund in refunds)
            {
                var itemAlreadyInList = response.Data.SingleOrDefault(x => x.UserApplicantId == refund.UserApplicantId);

                if (itemAlreadyInList == null)
                {
                    var item = new PaymentPendingModel
                    {
                        Id = response.Data.Count + 1,
                        UserApplicantId = refund.UserApplicantId,
                        UserApplicantDesc = refund.UserApplicant?.Name,
                        CurrencyId = refund.CurrencyId,
                        CurrencyDesc = refund.Currency?.Text,
                        Entities = new List<EntityToPay> { new EntityToPay
                        {
                            Id = refund.Id,
                            Type = "refund",
                            WorkflowId = refund.WorkflowId,
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

                    var tuple = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(refund.Id);

                    if (tuple.Item1.Any())
                        item.Ammount = tuple.Item1.Sum(x => x.TotalAmmount) - tuple.Item2.Sum(x => x.Ammount);
                    else
                        item.Ammount = refund.TotalAmmount;

                    if (refund.CurrencyExchange > 0) item.Ammount *= refund.CurrencyExchange;

                    response.Data.Add(item);
                }
                else
                {
                    itemAlreadyInList.Entities.Add(new EntityToPay
                    {
                        Id = refund.Id,
                        Type = "refund",
                        WorkflowId = refund.WorkflowId,
                        NextWorkflowStateId = settings.WorkflowStatusFinalizedId
                    });

                    var tuple = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(refund.Id);
                    decimal ammount = 0;

                    if (tuple.Item1.Any())
                        ammount = tuple.Item1.Sum(x => x.TotalAmmount) - tuple.Item2.Sum(x => x.Ammount);
                    else
                        ammount = refund.TotalAmmount;

                    if (refund.CurrencyExchange > 0)
                    {
                        var ammountInPesos = ammount * refund.CurrencyExchange;
                        itemAlreadyInList.Ammount += ammountInPesos;
                    }
                    else
                    {
                        itemAlreadyInList.Ammount += ammount;
                    }
                }
            }
        }

        private void FillAdvancements(Dictionary<string, string> employeeDicc, Dictionary<string, string> employeeManagerDicc, Dictionary<string, string> employeeNameManagerDicc, Response<IList<PaymentPendingModel>> response)
        {
            var advancements = unitOfWork.AdvancementRepository.GetAllPaymentPending(settings.WorkFlowStateAccounted);

            foreach (var advancement in advancements)
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
                        Entities = new List<EntityToPay> { new EntityToPay
                        {
                            Id = advancement.Id,
                            Type = "advancement",
                            WorkflowId = advancement.WorkflowId,
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
                        Type = "advancement",
                        WorkflowId = advancement.WorkflowId,
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
