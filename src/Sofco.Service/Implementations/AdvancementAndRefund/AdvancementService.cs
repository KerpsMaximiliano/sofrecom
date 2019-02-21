using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class AdvancementService : IAdvancementService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<AdvancementService> logger;
        private readonly IAdvancemenValidation validation;
        private readonly AppSetting settings;
        private readonly IUserData userData;

        public AdvancementService(IUnitOfWork unitOfWork,
            ILogMailer<AdvancementService> logger,
            IAdvancemenValidation validation,
            IUserData userData,
            IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.validation = validation;
            this.settings = settingOptions.Value;
            this.userData = userData;
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

                var workflow = unitOfWork.WorkflowRepository.GetLastByType(domain.Type == AdvancementType.Salary ? settings.SalaryWorkflowId : settings.ViaticumWorkflowId);

                domain.WorkflowId = workflow.Id;

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

            return response;
        }

        public Response<IList<AdvancementListItem>> GetAllInProcess()
        {
            var response = new Response<IList<AdvancementListItem>>();
            response.Data = new List<AdvancementListItem>();

            var employeeDicc = new Dictionary<string, string>();

            var currentUser = userData.GetCurrentUser();

            var hasAllAccess = HasAllAccess(currentUser);

            var advancements = unitOfWork.AdvancementRepository.GetAllInProcess();

            if (hasAllAccess)
            {
                response.Data = advancements.Select(x =>
                {
                    var item = new AdvancementListItem(x);

                    item.Bank = GetBank(x.UserApplicant.Email, employeeDicc);

                    return item;
                }).ToList();
            }
            else
            {
                //var readAccess = unitOfWork.AdvancementRepository.GetWorkflowReadAccess(settings.AdvacementWorkflowId);

                foreach (var advancement in advancements)
                {
                    //if (ValidateManagerAccess(advancement, transition, currentUser) || HasReadAccess(readAccess, currentUser))
                    if (ValidateManagerAccess(advancement, currentUser))
                    {
                        if (response.Data.All(x => x.Id != advancement.Id))
                        {
                            var item = new AdvancementListItem(advancement);

                            item.Bank = GetBank(advancement.UserApplicant.Email, employeeDicc);

                            response.Data.Add(item);
                        }
                    }
                }
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

        public Response<IList<PaymentPendingModel>> GetAllPaymentPending()
        {
            var response = new Response<IList<PaymentPendingModel>>();
            response.Data = new List<PaymentPendingModel>();

            var employeeDicc = new Dictionary<string, string>();

            var currentUser = userData.GetCurrentUser();

            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUser.Email);

            if (!hasDafGroup) return response;

            var advancements = unitOfWork.AdvancementRepository.GetAllPaymentPending(settings.WorkFlowStatePaymentPending);

            response.Data = advancements.Select(x =>
            {
                var item = new PaymentPendingModel
                {
                    Id = x.Id,
                    UserApplicantId = x.UserApplicantId,
                    UserApplicantDesc = x.UserApplicant?.Name,
                    CurrencyId = x.CurrencyId,
                    CurrencyDesc = x.Currency?.Text,
                    Ammount = x.Ammount,
                    WorkflowId = x.WorkflowId,
                    Type = "Adelanto"
                };

                item.Bank = GetBank(x.UserApplicant?.Email, employeeDicc);

                var transition = x.Status.ActualTransitions.FirstOrDefault();

                if (transition != null)
                {
                    item.NextWorkflowStateId = transition.NextWorkflowStateId;
                }

                return item;
            }).ToList();

            return response;
        }

        private string GetBank(string email, Dictionary<string, string> employeeDictionary)
        {
            if (!employeeDictionary.ContainsKey(email))
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmail(email);

                employeeDictionary.Add(email, employee?.Bank);
            }

            return employeeDictionary[email];
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

            var currentUser = userData.GetCurrentUser();

            var hasAllAccess = HasAllAccess(currentUser);

            var advancements = unitOfWork.AdvancementRepository.GetAllFinalized(settings.WorkflowStatusDraft, model);

            if (hasAllAccess)
            {
                response.Data = advancements.Select(x =>
                {
                    var item = new AdvancementListItem(x);

                    item.Bank = GetBank(x.UserApplicant.Email, employeeDicc);

                    return item;
                }).ToList();
            }
            else
            {
                foreach (var advancement in advancements)
                {
                    if (ValidateManagerAccess(advancement, currentUser))
                    {
                        if (response.Data.All(x => x.Id != advancement.Id))
                        {
                            var item = new AdvancementListItem(advancement);

                            item.Bank = GetBank(advancement.UserApplicant.Email, employeeDicc);

                            response.Data.Add(item);
                        }
                    }
                }
            }

            if (!response.Data.Any())
            {
                response.AddWarning(Resources.Common.SearchNotFound);
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

        public Response<IList<AdvancementUnrelatedItem>> GetUnrelated()
        {
            var currentUser = userData.GetCurrentUser();

            var advancements = unitOfWork.AdvancementRepository.GetUnrelated(currentUser.Id, settings.WorkflowStatusDraft);

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
                    Text = $"{advancement.CreationDate:dd/MM/yyyy} - {advancement.Ammount} {advancement.Currency.Text}"
                });
            }

            return response;
        }

        private bool HasAllAccess(UserLiteModel currentUser)
        {
            var hasDirectorGroup = unitOfWork.UserRepository.HasDirectorGroup(currentUser.Email);
            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUser.Email);
            var hasRrhhGroup = unitOfWork.UserRepository.HasRrhhGroup(currentUser.Email);
            var hasGafGroup = unitOfWork.UserRepository.HasGafGroup(currentUser.Email);

            var hasAllAccess = hasDirectorGroup || hasRrhhGroup || hasGafGroup || hasDafGroup;
            return hasAllAccess;
        }

        private bool HasReadAccess(IList<WorkflowReadAccess> readAccess, UserLiteModel currentUser)
        {
            var hasAccess = false;

            foreach (var workflowReadAccess in readAccess)
            {
                hasAccess = ValidateUserReadAccess(currentUser, workflowReadAccess.UserSource, hasAccess);
                hasAccess = ValidateGroupAccess(currentUser, workflowReadAccess.UserSource, hasAccess);
            }

            return hasAccess;
        }

        private bool ValidateUserReadAccess(UserLiteModel currentUser, UserSource userSource, bool hasAccess)
        {
            if (userSource.Code == settings.UserUserSource && userSource.SourceId == currentUser.Id)
            {
                hasAccess = true;
            }

            return hasAccess;
        }

        private bool ValidateGroupAccess(UserLiteModel currentUser, UserSource userSource, bool hasAccess)
        {
            if (userSource.Code == settings.GroupUserSource)
            {
                var user = unitOfWork.UserRepository.GetSingleWithUserGroup(x => x.Id == currentUser.Id);
                var groups = user.UserGroups?.Select(x => x.Group).Distinct().ToList();

                if (groups != null && groups.Any())
                {
                    if (groups.Any(u => u.Id == userSource.SourceId))
                    {
                        hasAccess = true;
                    }
                }
            }

            return hasAccess;
        }

        private bool ValidateManagerAccess(Advancement entity, UserLiteModel currentUser)
        {
            if (entity.AuthorizerId.HasValue && entity.AuthorizerId.Value == currentUser.Id)
            {
                return true;
            }
            else
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                if (employee.ManagerId.HasValue && employee.Manager != null && employee.ManagerId.Value == currentUser.Id)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
