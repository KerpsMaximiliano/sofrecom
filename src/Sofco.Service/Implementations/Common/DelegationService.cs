using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Common;
using Sofco.Core.Services.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Common
{
    public class DelegationService : IDelegationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<DelegationService> logger;
        private readonly IUserData userData;
        private readonly AppSetting appSetting;

        public DelegationService(IUnitOfWork unitOfWork, ILogMailer<DelegationService> logger, IUserData userData, IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            this.appSetting = appSettingOptions.Value;
        }

        public Response Add(DelegationAddModel model)
        {
            var currentUser = userData.GetCurrentUser();

            var response = new Response();

            if (model.Type == DelegationType.RefundAdd)
            {
                model.UserSourceId = currentUser.Id;
            }

            Validate(model, response, currentUser.Id);

            if (response.HasErrors()) return response;

            try
            {
                var userDelegate = new Delegation
                {
                    GrantedUserId = model.GrantedUserId,
                    UserId = currentUser.Id,
                    Type = model.Type.GetValueOrDefault(),
                    AnalyticSourceId = model.AnalyticSourceId,
                    UserSourceId = model.UserSourceId,
                    SourceType = model.SourceType,
                    Created = DateTime.UtcNow
                };

                if (model.UserSourceId.HasValue && model.UserSourceId.Value > 0)
                {
                    var user = userData.GetById(model.UserSourceId.Value);

                    var employee = unitOfWork.EmployeeRepository.GetByEmail(user.Email);

                    if (employee != null) userDelegate.EmployeeSourceId = employee.Id;
                }

                unitOfWork.DelegationRepository.Insert(userDelegate);

                HandleAddDelegate(userDelegate);

                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Delegation.AddSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void HandleAddDelegate(Delegation userDelegate)
        {
            if (userDelegate.Type == DelegationType.ManagementReport)
            {
                var group = unitOfWork.GroupRepository.GetByCode(appSetting.ManagementReportDelegateRole);

                if (!unitOfWork.UserGroupRepository.ExistById(userDelegate.GrantedUserId, group.Id))
                {
                    var userRole = new UserGroup
                    {
                        GroupId = group.Id,
                        UserId = userDelegate.GrantedUserId
                    };

                    unitOfWork.UserGroupRepository.Insert(userRole);
                }
            }
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var userDelegate = unitOfWork.DelegationRepository.Get(id);

            if (userDelegate == null)
            {
                response.AddError(Resources.Admin.Delegation.NotFound);
                return response;
            }

            try
            {
                unitOfWork.DelegationRepository.Delete(userDelegate);

                HandleDeleteDelegate(userDelegate);

                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Delegation.DeleteSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void HandleDeleteDelegate(Delegation userDelegate)
        {
            if (userDelegate.Type == DelegationType.ManagementReport)
            {
                var group = unitOfWork.GroupRepository.GetByCode(appSetting.ManagementReportDelegateRole);

                if (unitOfWork.UserGroupRepository.ExistById(userDelegate.GrantedUserId, group.Id))
                {
                    var userRole = new UserGroup
                    {
                        GroupId = group.Id,
                        UserId = userDelegate.GrantedUserId
                    };

                    unitOfWork.UserGroupRepository.Delete(userRole);
                }
            }
        }

        public Response<IList<DelegationModel>> GetByUserId()
        {
            var currentUser = userData.GetCurrentUser();

            var response = new Response<IList<DelegationModel>> { Data = new List<DelegationModel>() };

            var delegates = unitOfWork.DelegationRepository.GetByUserId(currentUser.Id);

            foreach (var userDelegate in delegates)
            {
                var model = new DelegationModel(userDelegate);
                HandleGetDelegate(userDelegate, model);
                response.Data.Add(model);
            }

            return response;
        }

        public Response<IList<AnalyticsWithEmployees>> GetAnalytics()
        {
            var currentUser = userData.GetCurrentUser();

            var response = new Response<IList<AnalyticsWithEmployees>> { Data = new List<AnalyticsWithEmployees>() };

            var delegates = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.ManagementReport);

            var list = unitOfWork.AnalyticRepository.GetByManagerIdAndDirectorId(currentUser.Id).ToList();

            foreach (var analytic in list)
            {
                var itemToAdd = new AnalyticsWithEmployees
                {
                    Id = analytic.Id,
                    Text = $"{analytic.Title} - {analytic.Name}",
                    Resources = new List<ResourceOption>()
                };

                var resources = unitOfWork.EmployeeRepository.GetByAnalyticIdInCurrentDate(analytic.Id).ToList();

                foreach (var resource in resources)
                {
                    var user = unitOfWork.UserRepository.GetByEmail(resource.Email);

                    if (user != null)
                    {
                        itemToAdd.Resources.Add(new ResourceOption { Id = resource.Id, Text = resource.Name, UserId = user.Id });
                    }
                }

                response.Data.Add(itemToAdd);
            }

            foreach (var userDelegate in delegates)
            {
                if (userDelegate.SourceType == DelegationSourceType.Analytic)
                {
                    if (response.Data.All(x => x.Id != userDelegate.AnalyticSourceId.GetValueOrDefault()))
                    {
                        var analytic = unitOfWork.AnalyticRepository.GetAnalyticLiteById(userDelegate.AnalyticSourceId.GetValueOrDefault());

                        var itemToAdd = new AnalyticsWithEmployees
                        {
                            Id = analytic.Id,
                            Text = $"{analytic.Title} - {analytic.Name}",
                            Resources = new List<ResourceOption>()
                        };

                        var resources = unitOfWork.EmployeeRepository.GetByAnalyticIdInCurrentDate(analytic.Id).ToList();

                        foreach (var resource in resources)
                        {
                            var user = unitOfWork.UserRepository.GetByEmail(resource.Email);

                            if (user != null)
                            {
                                itemToAdd.Resources.Add(new ResourceOption { Id = resource.Id, Text = resource.Name, UserId = user.Id });
                            }
                        }

                        response.Data.Add(itemToAdd);
                    }
                }

                if (userDelegate.SourceType == DelegationSourceType.User)
                {
                    var user = unitOfWork.UserRepository.GetUserLiteById(userDelegate.UserSourceId.GetValueOrDefault());
                    var analyticId = userDelegate.AnalyticSourceId.GetValueOrDefault();
                    var employee = unitOfWork.EmployeeRepository.GetByEmail(user.Email);

                    if (response.Data.All(x => x.Id != analyticId))
                    {
                        var analytic = unitOfWork.AnalyticRepository.GetAnalyticLiteById(analyticId);

                        var itemToAdd = new AnalyticsWithEmployees
                        {
                            Id = analytic.Id,
                            Text = $"{analytic.Title} - {analytic.Name}",
                            Resources = new List<ResourceOption>()
                        };

                        itemToAdd.Resources.Add(new ResourceOption { Id = employee.Id, Text = employee.Name, UserId = user.Id });

                        response.Data.Add(itemToAdd);
                    }
                    else
                    {
                        var analytic = response.Data.FirstOrDefault(x => x.Id == analyticId);

                        analytic?.Resources.Add(new ResourceOption { Id = employee.Id, Text = employee.Name, UserId = user.Id });
                    }
                }
            }

            return response;
        }

        public Response<IList<Option>> GetResources()
        {
            var currentUser = userData.GetCurrentUser();

            var response = new Response<IList<Option>>();

            var employees = unitOfWork.EmployeeRepository.GetByManagerId(currentUser.Id);

            var users = unitOfWork.UserRepository.GetByEmail(employees.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(x => x.Email).ToList());

            response.Data = users.Select(x => new Option {Id = x.Id, Text = x.Name}).ToList();

            return response;
        }

        private void HandleGetDelegate(Delegation userDelegate, DelegationModel model)
        {
            if (userDelegate.Type == DelegationType.ManagementReport)
            {
                var analytic = unitOfWork.AnalyticRepository.Get(userDelegate.AnalyticSourceId.GetValueOrDefault());

                if (analytic != null)
                {
                    model.AnalyticSourceName = $"{analytic.Title} - {analytic.Name}";
                }

                if (userDelegate.SourceType == DelegationSourceType.User)
                {
                    var user = unitOfWork.UserRepository.Get(userDelegate.UserSourceId.GetValueOrDefault());

                    if (user != null)
                    {
                        model.UserSourceName = user.Name;
                    }
                }
                else
                {
                    model.UserSourceName = "Todos";
                }
            }

            if (userDelegate.Type == DelegationType.Advancement || userDelegate.Type == DelegationType.LicenseAuthorizer)
            {
                if (userDelegate.UserSourceId.HasValue && userDelegate.UserSourceId.Value > 0)
                {
                    var user = unitOfWork.UserRepository.Get(userDelegate.UserSourceId.GetValueOrDefault());

                    if (user != null)
                    {
                        model.UserSourceName = user.Name;
                    }
                }
                else
                {
                    model.UserSourceName = "Todos";
                }
            }

            if (userDelegate.Type == DelegationType.Solfac || userDelegate.Type == DelegationType.PurchaseOrderActive)
            {
                var analytic = unitOfWork.AnalyticRepository.Get(userDelegate.AnalyticSourceId.GetValueOrDefault());

                if (analytic != null)
                {
                    model.AnalyticSourceName = $"{analytic.Title} - {analytic.Name}";
                }
            }


            if (userDelegate.Type == DelegationType.PurchaseOrderApprovalCommercial)
            {
                var area = unitOfWork.AreaRepository.Get(userDelegate.AnalyticSourceId.GetValueOrDefault());

                if (area != null)
                {
                    model.AnalyticSourceName = area.Text;
                }
            }

            if (userDelegate.Type == DelegationType.PurchaseOrderApprovalOperation)
            {
                var sector = unitOfWork.SectorRepository.Get(userDelegate.AnalyticSourceId.GetValueOrDefault());

                if (sector != null)
                {
                    model.AnalyticSourceName = sector.Text;
                }
            }

            if (userDelegate.Type == DelegationType.RefundApprovall)
            {
                var analytic = unitOfWork.AnalyticRepository.Get(userDelegate.AnalyticSourceId.GetValueOrDefault());

                if (analytic != null)
                {
                    model.AnalyticSourceName = $"{analytic.Title} - {analytic.Name}";
                }

                model.UserSourceName = "Todos";
            }

            if (userDelegate.Type == DelegationType.RefundAdd || userDelegate.Type == DelegationType.WorkTime)
            {
                var analytic = unitOfWork.AnalyticRepository.Get(userDelegate.AnalyticSourceId.GetValueOrDefault());

                if (analytic != null)
                {
                    model.AnalyticSourceName = $"{analytic.Title} - {analytic.Name}";
                }

                var user = unitOfWork.UserRepository.Get(userDelegate.UserSourceId.GetValueOrDefault());

                if (user != null)
                {
                    model.UserSourceName = user.Name;
                }
            }
        }

        private void Validate(DelegationAddModel model, Response response, int currentUserId)
        {
            if (model == null)
            {
                response.AddError(Resources.Admin.Delegation.ModelNull);
                return;
            }

            if (!unitOfWork.UserRepository.ExistById(model.GrantedUserId))
                response.AddError(Resources.Admin.Delegation.GrantedUserNotFound);

            if (!model.Type.HasValue)
                response.AddError(Resources.Admin.Delegation.TypeRequired);

            if (model.Type.HasValue && (model.Type.Value == DelegationType.ManagementReport || model.Type.Value == DelegationType.Solfac))
            {
                if (!model.AnalyticSourceId.HasValue || model.AnalyticSourceId == 0)
                    response.AddError(Resources.Admin.Delegation.AnalyticRequired);
            }

            if (response.HasErrors()) return;

            if (unitOfWork.DelegationRepository.Exist(model, currentUserId))
                response.AddError(Resources.Admin.Delegation.AlreadyExist);

            if (currentUserId == model.GrantedUserId)
                response.AddError(Resources.Admin.Delegation.UserEqualsGrantedUser);

            if (model.SourceType == DelegationSourceType.User && model.GrantedUserId == model.UserSourceId)
                response.AddError(Resources.Admin.Delegation.UserSourceEqualsGrantedUser);
        }
    }
}
