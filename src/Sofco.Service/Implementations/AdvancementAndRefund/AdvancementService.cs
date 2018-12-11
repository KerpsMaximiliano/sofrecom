using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
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
                domain.CreationDate = DateTime.UtcNow.Date;
                domain.StatusId = settings.WorkflowStatusDraft;

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
                var advancement = unitOfWork.AdvancementRepository.GetById(model.Id);
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

            var currentUser = userData.GetCurrentUser();

            var hasAllAccess = HasAllAccess(currentUser);

            var advancements = unitOfWork.AdvancementRepository.GetAllInProcess();

            if (hasAllAccess)
            {
                response.Data = advancements.Select(x => new AdvancementListItem(x)).ToList();
            }
            else
            {
                //var readAccess = unitOfWork.AdvancementRepository.GetWorkflowReadAccess(settings.AdvacementWorkflowId);

                foreach (var advancement in advancements)
                {
                    foreach (var transition in advancement.Status.ActualTransitions)
                    {
                        //if (ValidateManagerAccess(advancement, transition, currentUser) || HasReadAccess(readAccess, currentUser))
                        if (ValidateManagerAccess(advancement, transition, currentUser))
                        {
                            if (response.Data.All(x => x.Id != advancement.Id))
                            {
                                response.Data.Add(new AdvancementListItem(advancement));
                            }
                        }
                    }
                }
            }

            return response;
        }

        public Response<IList<AdvancementHistoryModel>> GetHistories(int id)
        {
            var histories = unitOfWork.AdvancementRepository.GetHistories(id);

            var response = new Response<IList<AdvancementHistoryModel>>();

            response.Data = histories.Select(x => new AdvancementHistoryModel(x)).ToList();

            return response;
        }

        public Response<IList<AdvancementListItem>> GetAllFinalized(AdvancementSearchFinalizedModel model)
        {
            var response = new Response<IList<AdvancementListItem>>();
            response.Data = new List<AdvancementListItem>();

            var currentUser = userData.GetCurrentUser();

            var hasAllAccess = HasAllAccess(currentUser);

            var advancements = unitOfWork.AdvancementRepository.GetAllFinalized(settings.WorkflowStatusDraft, model);

            if (hasAllAccess)
            {
                response.Data = advancements.Select(x => new AdvancementListItem(x)).ToList();
            }
            else
            {
                foreach (var advancement in advancements)
                {
                    foreach (var transition in advancement.Status.ActualTransitions)
                    {
                        if (ValidateManagerAccess(advancement, transition, currentUser))
                        {
                            if (response.Data.All(x => x.Id != advancement.Id))
                            {
                                response.Data.Add(new AdvancementListItem(advancement));
                            }
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

            var analytics = unitOfWork.AnalyticRepository.GetAnalyticsByEmployee(employee.Id);

            var i = 0;

            while (!response.Data && i < analytics.Count)
            {
                if (employee.ManagerId.HasValue)
                {
                    if (analytics[i].ManagerId.GetValueOrDefault() == employee.ManagerId)
                    {
                        response.Data = true;
                    }

                    if (analytics[i].Sector.ResponsableUserId == employee.ManagerId)
                    {
                        response.Data = true;
                    }
                }

                i++;
            }

            if (!response.Data)
            {
                response.AddError(Resources.AdvancementAndRefund.Advancement.CannotLoad);
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

        private bool ValidateManagerAccess(Advancement entity, WorkflowStateTransition transition, UserLiteModel currentUser)
        {
            if (transition != null && transition.WorkflowStateAccesses != null && transition.WorkflowStateAccesses.Any(x => x.UserSource.Code == settings.ManagerUserSource))
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
            }

            return false;
        }
    }
}
