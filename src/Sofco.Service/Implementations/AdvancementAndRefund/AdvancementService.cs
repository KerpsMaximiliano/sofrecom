﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;
using Spire.Pdf;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class AdvancementService : IAdvancementService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<AdvancementService> logger;
        private readonly IAdvancemenValidation validation;
        private readonly AppSetting settings;
        private readonly FileConfig fileConfig;
        private readonly IUserData userData;
        private readonly IRoleManager roleManager;
        private readonly IWorkflowStateRepository workflowStateRepository;
        private readonly IAdvancementFileManager advancementFileManager;

        public AdvancementService(IUnitOfWork unitOfWork,
            ILogMailer<AdvancementService> logger,
            IAdvancemenValidation validation,
            IWorkflowStateRepository workflowStateRepository,
            IOptions<FileConfig> fileOptions,
            IUserData userData,
            IRoleManager roleManager,
            IAdvancementFileManager advancementFileManager,
            IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.workflowStateRepository = workflowStateRepository;
            this.validation = validation;
            this.roleManager = roleManager;
            this.fileConfig = fileOptions.Value;
            this.settings = settingOptions.Value;
            this.userData = userData;
            this.advancementFileManager = advancementFileManager;
        }

        public Response<string> Add(AdvancementModel model)
        {
            var response = new Response<string>();

            validation.ValidateAdd(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain();
                domain.CreationDate = DateTime.UtcNow;
                domain.StatusId = settings.WorkflowStatusDraft;

                Domain.Models.Workflow.Workflow workflow = new Domain.Models.Workflow.Workflow();

                if (domain.Type == AdvancementType.Salary)
                    workflow = unitOfWork.WorkflowRepository.GetLastByType(settings.SalaryWorkflowId);

                if (domain.Type == AdvancementType.Viaticum)
                {
                    if (domain.PaymentForm == AdvancementPaymentForm.Cash)
                        workflow = unitOfWork.WorkflowRepository.GetLastByType(settings.CashWorkflowId);
                    else
                        workflow = unitOfWork.WorkflowRepository.GetLastByType(settings.ViaticumWorkflowId);
                }

                domain.WorkflowId = workflow.Id;
                domain.InWorkflowProcess = true;

                unitOfWork.AdvancementRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Advancement.AddSuccess);

                response.Data = domain.Id.ToString();
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public Response Update(AdvancementModel model)
        {
            var response = validation.ValidateUpdate(model);

            if (response.HasErrors()) return response;

            try
            {
                var advancement = unitOfWork.AdvancementRepository.Get(model.Id);
                model.UpdateDomain(advancement);

                unitOfWork.AdvancementRepository.Update(advancement);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Advancement.UpdateSuccess);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public Response<AdvancementEditModel> Get(int id)
        {
            var response = new Response<AdvancementEditModel>();

            var advancement = unitOfWork.AdvancementRepository.GetFullById(id);

            if (advancement == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.NotFound);
                return response;
            }

            response.Data = new AdvancementEditModel(advancement);

            var employee = unitOfWork.EmployeeRepository.GetByEmail(advancement.UserApplicant.Email);

            if (employee != null)
            {
                response.Data.Office = employee.OfficeAddress;
                response.Data.Bank = employee.Bank;
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var domain = unitOfWork.AdvancementRepository.Get(id);

            if (domain == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.NotFound);
                return response;
            }

            if (domain.StatusId != settings.WorkflowStatusDraft)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.CannotDelete);
                return response;
            }

            try
            {
                unitOfWork.AdvancementRepository.Delete(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Advancement.DeleteSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<AdvancementRefundModel> GetResume(IList<int> ids)
        {
            var data = unitOfWork.AdvancementRepository.GetAdvancementsAndRefundsByAdvancementId(ids);

            var response = new Response<AdvancementRefundModel> { Data = new AdvancementRefundModel() };

            response.Data.Refunds = data.Item1.Select(x => new RefundRelatedModel
            {
                Id = x.Id,
                Analytic = x.Analytic?.Name,
                LastRefund = x.LastRefund,
                CashReturn = x.CashReturn,
                Total = x.TotalAmmount,
                StatusName = x.Status?.Name,
                StatusType = x.Status?.Type,
            })
            .ToList();

            response.Data.Advancements = data.Item2.Select(x => new AdvancementRelatedModel
            {
                Id = x.Id,
                Total = x.Ammount
            })
            .ToList();

            return response;
        }

        public Response<IList<Option>> GetStates()
        {
            var salaryAdvs = workflowStateRepository.GetStateByWorkflowTypeCode(settings.SalaryWorkflowTypeCode);
            var viaticumAdvs = workflowStateRepository.GetStateByWorkflowTypeCode(settings.ViaticumWorkflowTypeCode);

            var result = salaryAdvs.Union(viaticumAdvs).Distinct().Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();

            if (result.All(x => x.Id != settings.WorkflowStatusFinalizedId))
            {
                var finalizeState = workflowStateRepository.Get(settings.WorkflowStatusFinalizedId);

                result.Add(new Option { Id = finalizeState.Id, Text = finalizeState.Name });
            }

            var draft = result.SingleOrDefault(x => x.Id == settings.WorkflowStatusDraft);

            if (draft != null) result.Remove(draft);

            return new Response<IList<Option>>
            {
                Data = result
            };
        }

        public Response IsCashEnable()
        {
            var response = new Response();

            var workflow = unitOfWork.WorkflowRepository.GetLastByType(settings.CashWorkflowId);

            if (workflow == null)
            {
                response.AddError(Resources.Workflow.Workflow.CannotDoTransition);
                return response;
            }

            var transitions = unitOfWork.WorkflowRepository.GetTransitionByWorkflowAndState(workflow.Id, settings.WorkflowStatusDraft);

            var currentUser = userData.GetCurrentUser();

            if (transitions.Any(x => x.WorkflowStateAccesses.Any(s =>
                s.UserSource.Code == settings.UserUserSource && s.UserSource.SourceId == currentUser.Id)))
            {
                return response;
            }
            else
            {
                response.AddError(Resources.Workflow.Workflow.CannotDoTransition);
            }

            return response;
        }

        private void SetBankAndManager(string email, Dictionary<string, string> employeeDictionary,
            Dictionary<string, string> employeeManagerDictionary, Dictionary<string, string> employeeNameDictionary)
        {
            if (!employeeDictionary.ContainsKey(email))
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmail(email);

                employeeDictionary.Add(email, employee?.Bank);
                employeeManagerDictionary.Add(email, employee?.Manager?.Name);
                employeeNameDictionary.Add(email, employee?.Name);
            }
        }

        public Response<IList<WorkflowHistoryModel>> GetHistories(int id)
        {
            var histories = unitOfWork.AdvancementRepository.GetHistories(id);

            var response = new Response<IList<WorkflowHistoryModel>>();

            response.Data = histories.Select(x => new WorkflowHistoryModel(x)).ToList();

            return response;
        }

        public Response<IList<AdvancementListItem>> GetAllFinalized(AdvancementSearchFinalizedModel model)
        {
            var response = new Response<IList<AdvancementListItem>>();
            response.Data = new List<AdvancementListItem>();

            var employeeDicc = new Dictionary<string, string>();
            var employeeManagerDicc = new Dictionary<string, string>();
            var employeeNameDicc = new Dictionary<string, string>();

            var currentUser = userData.GetCurrentUser();

            var hasAllAccess = roleManager.HasAdvancementAccess();

            var advancements = unitOfWork.AdvancementRepository.GetAllFinalized(model, settings.WorkflowStatusDraft);

            if (hasAllAccess)
            {
                response.Data = advancements.Select(x =>
                {
                    var item = new AdvancementListItem(x);

                    if (x.Histories.Any())
                    {
                        var lastHistory = x.Histories.OrderByDescending(s => s.CreatedDate).FirstOrDefault();

                        if (lastHistory != null) item.LastUpdate = lastHistory.CreatedDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
                    }
                    else
                    {
                        item.LastUpdate = x.CreationDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
                    }

                    SetBankAndManager(x.UserApplicant?.Email, employeeDicc, employeeManagerDicc, employeeNameDicc);

                    if (x.UserApplicant != null)
                    {
                        item.Bank = employeeDicc[x.UserApplicant?.Email];
                        item.Manager = employeeManagerDicc[x.UserApplicant?.Email];
                        item.UserApplicantDesc = employeeNameDicc[x.UserApplicant?.Email];
                    }

                    return item;
                }).ToList();
            }
            else
            {
                foreach (var advancement in advancements)
                {
                    if (ValidateManagerAccess(advancement, currentUser) || ValidateSectorAccess(advancement, currentUser) || ValidateUserAccess(advancement, currentUser))
                    {
                        if (response.Data.All(x => x.Id != advancement.Id))
                        {
                            var item = new AdvancementListItem(advancement);

                            if (advancement.Histories.Any())
                            {
                                var lastHistory = advancement.Histories.OrderByDescending(s => s.CreatedDate).FirstOrDefault();

                                if (lastHistory != null) item.LastUpdate = lastHistory.CreatedDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
                            }
                            else
                            {
                                item.LastUpdate = advancement.CreationDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
                            }

                            SetBankAndManager(advancement.UserApplicant?.Email, employeeDicc, employeeManagerDicc, employeeNameDicc);

                            if (advancement.UserApplicant != null)
                            {
                                item.Bank = employeeDicc[advancement.UserApplicant?.Email];
                                item.Manager = employeeManagerDicc[advancement.UserApplicant?.Email];
                                item.UserApplicantDesc = employeeNameDicc[advancement.UserApplicant?.Email];
                            }

                            response.Data.Add(item);
                        }
                    }
                }
            }

            if (!response.Data.Any())
            {
                response.AddWarning(Resources.Common.SearchNotFound);
            }

            if (!string.IsNullOrEmpty(model.Bank))
            {
                response.Data = response.Data.Where(d => d.Bank == model.Bank).ToList();
            }
            return response;

        }

        public Response<bool> CanLoad()
        {
            var response = new Response<bool> { Data = false };

            var user = userData.GetCurrentUser();

            var employee = unitOfWork.EmployeeRepository.GetByEmail(user.Email);

            var managerId = employee.ManagerId;

            var analytics = unitOfWork.AnalyticRepository.GetAllOpenReadOnly();

            var sectors = unitOfWork.SectorRepository.GetAll();

            if (analytics.Any(s => s.ManagerId == managerId)
                || sectors.Any(s => s.ResponsableUserId == managerId))
            {
                response.Data = true;

                return response;
            }

            return response;
        }

        public Response<IList<AdvancementUnrelatedItem>> GetUnrelated(int userId)
        {
            var advancements = unitOfWork.AdvancementRepository.GetUnrelated(userId, settings.WorkflowStatusApproveId);

            var response = new Response<IList<AdvancementUnrelatedItem>>();
            response.Data = new List<AdvancementUnrelatedItem>();

            foreach (var advancement in advancements)
            {
                response.Data.Add(new AdvancementUnrelatedItem
                {
                    Id = advancement.Id,
                    CurrencyId = advancement.CurrencyId,
                    CurrencyText = advancement.Currency.Text,
                    Ammount = advancement.Ammount,
                    HasLastRefundMarked = advancement.AdvancementRefunds.Any(x => x.Refund.LastRefund),
                    Text = $"#{advancement.Id} - {advancement.CreationDate:dd/MM/yyyy} - {advancement.Ammount} {advancement.Currency.Text}"
                });
            }


            return response;
        }

        public Response<byte[]> GetFile(int id)
        {
            var response = new Response<byte[]>();

            try
            {
                var domain = unitOfWork.AdvancementRepository.GetFullByIdForZip(id);

                if (domain == null)
                {
                    response.AddError(Resources.AdvancementAndRefund.Refund.NotFound);
                    return response;
                }

                var employee = unitOfWork.EmployeeRepository.GetByEmail(domain.UserApplicant.Email);

                var responseResume = new Response<AdvancementRefundModel>();

                if (domain.AdvancementRefunds != null && domain.AdvancementRefunds.Any())
                {
                    responseResume = GetResume(domain.AdvancementRefunds.Select(x => x.AdvancementId).ToList());
                }

                var excel = advancementFileManager.CreateExcel(domain, employee, responseResume.Data);

                response.Data = excel.GetAsByteArray();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.GeneralError);
            }
            
            return response;
        }

        private bool ValidateUserAccess(Advancement entity, UserLiteModel currentUser)
        {
            bool hasAccess = entity.Status.ActualTransitions.Any(x => x.WorkflowStateAccesses.Any(s => s.UserSource.SourceId == currentUser.Id && s.UserSource.Code == settings.UserUserSource && s.AccessDenied == false));

            return hasAccess;
        }

        private bool ValidateManagerAccess(Advancement entity, UserLiteModel currentUser)
        {
            var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

            if (employee.ManagerId.HasValue && employee.Manager != null && employee.ManagerId.Value == currentUser.Id)
            {
                return true;
            }

            var delegations = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.Advancement);

            if (delegations.Any(x => employee.ManagerId != null && (x.UserId == employee.ManagerId.Value && (x.UserSourceId == null || x.UserSourceId == entity.UserApplicantId))))
            {
                return true;
            }

            return false;
        }

        private bool ValidateSectorAccess(Advancement entity, UserLiteModel currentUser)
        {
            var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

            var sectors = unitOfWork.EmployeeRepository.GetAnalyticsWithSector(employee.Id);

            if (sectors.Any(x => x.ResponsableUserId == currentUser.Id))
            {
                return true;
            }

            return false;
        }
    }
}
