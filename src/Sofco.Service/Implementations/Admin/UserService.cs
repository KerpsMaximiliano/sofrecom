using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.Admin;
using Sofco.Domain.Utils;
using Sofco.Core.Services.Admin;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Relationships;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.AllocationManagement;

namespace Sofco.Service.Implementations.Admin
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogMailer<UserService> logger;

        private readonly ISessionManager sessionManager;

        private readonly IMapper mapper;

        private readonly AppSetting appSetting;

        private readonly IMailSender mailSender;

        private readonly IMailBuilder mailBuilder;

        private readonly EmailConfig emailConfig;

        public UserService(IUnitOfWork unitOfWork, 
            ILogMailer<UserService> logger, 
            ISessionManager sessionManager, 
            IMailSender mailSender,
            IMailBuilder mailBuilder,
            IOptions<AppSetting> appSettingsOptions,
            IOptions<EmailConfig> emailConfigOptions,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.sessionManager = sessionManager;
            this.mapper = mapper;
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            this.emailConfig = emailConfigOptions.Value;
            this.appSetting = appSettingsOptions.Value;
        }

        public Response<User> Active(int id, bool active)
        {
            var response = new Response<User>();
            var entity = unitOfWork.UserRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                if (active)
                {
                    entity.StartDate = DateTime.Now;
                    entity.EndDate = null;
                }
                else
                {
                    entity.EndDate = DateTime.Now;
                }

                unitOfWork.UserRepository.Update(entity);
                unitOfWork.Save();

                response.Data = entity;
                response.AddSuccess(active ? Resources.Admin.User.Enabled : Resources.Admin.User.Disabled);
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
            return response;
        }

        public Response<User> AddUserGroup(int userId, int groupId)
        {
            var response = new Response<User>();

            var user = unitOfWork.UserRepository.GetSingleWithUserGroup(x => x.Id == userId);

            if (user == null)
            {
                response.AddError(Resources.Admin.User.NotFound);
                return response;
            }

            var group = unitOfWork.GroupRepository.GetSingle(x => x.Id == groupId);

            if (group == null)
            {
                response.AddError(Resources.Admin.Group.NotFound);
                return response;
            }

            var entity = new UserGroup { UserId = userId, GroupId = groupId };

            unitOfWork.UserGroupRepository.Insert(entity);

            unitOfWork.Save();

            response.AddSuccess(Resources.Admin.User.GroupAssigned);

            AddLogSensibleData(group, user);

            return response;
        }

        private void AddLogSensibleData(Group group, User user)
        {
            if (appSetting.SensibleFunctionality == group.Code)
            {
                try
                {
                    var log = new Log
                    {
                        Created = DateTime.UtcNow,
                        Username = sessionManager.GetUserName(),
                        Comment = string.Format(Resources.Mails.MailMessageResource.SensibleData, user.Name)
                    };

                    unitOfWork.LogRepository.Insert(log);
                    unitOfWork.Save();

                    var rrhhGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.RRhhAb);
                    var monica = unitOfWork.UserRepository.Get(appSetting.MonicaBimanUserId);

                    var recipientsList = new List<string>();

                    if (rrhhGroup != null) recipientsList.Add(rrhhGroup.Email);
                    if (monica != null) recipientsList.Add(monica.Email);

                    var data = new MailDefaultData
                    {
                        Title = Resources.Mails.MailSubjectResource.SensibleData,
                        Message = string.Format(Resources.Mails.MailMessageResource.SensibleData, user.Name),
                        Recipients = recipientsList.Distinct().ToList()
                    };

                    var email = mailBuilder.GetEmail(data);

                    mailSender.Send(email);
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                }
            }
        }

        public IList<User> GetAllReadOnly(bool active)
        {
            if (active)
                return unitOfWork.UserRepository.GetAllActivesReadOnly();
            else
                return unitOfWork.UserRepository.GetAllReadOnly();
        }

        public Response<User> GetById(int id)
        {
            var response = new Response<User>();
            var entity = unitOfWork.UserRepository.GetSingleWithUserGroup(x => x.Id == id);

            if (entity != null)
            {
                response.Data = entity;
                return response;
            }

            response.AddError(Resources.Admin.User.NotFound);
            return response;
        }

        public Response<User> RemoveUserGroup(int userId, int groupId)
        {
            var response = new Response<User>();

            var userExist = unitOfWork.UserRepository.ExistById(userId);

            if (!userExist)
            {
                response.AddError(Resources.Admin.User.NotFound);
                return response;
            }

            var groupExist = unitOfWork.GroupRepository.ExistById(groupId);

            if (!groupExist)
            {
                response.AddError(Resources.Admin.Group.NotFound);
                return response;
            }

            try
            {
                var entity = new UserGroup { UserId = userId, GroupId = groupId };

                unitOfWork.UserGroupRepository.Delete(entity);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.User.GroupRemoved);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public Response<User> ChangeUserGroups(int userId, List<int> groupsToAdd, List<int> groupsToRemove)
        {
            var response = new Response<User>();

            var userExist = unitOfWork.UserRepository.Get(userId);

            if (userExist == null)
            {
                response.AddError(Resources.Admin.User.NotFound);
                return response;
            }

            Group groupSensible = null;

            try
            {
                foreach (var groupId in groupsToAdd)
                {
                    var entity = new UserGroup { UserId = userId, GroupId = groupId };
                    var groupExist = unitOfWork.GroupRepository.Get(groupId);
                    var userGroupExist = unitOfWork.UserGroupRepository.ExistById(userId, groupId);

                    if (groupExist != null && !userGroupExist)
                    {
                        unitOfWork.UserGroupRepository.Insert(entity);

                        if (appSetting.SensibleFunctionality == groupExist.Code)
                        {
                            groupSensible = groupExist;
                        }
                    }
                }

                foreach (var groupId in groupsToRemove)
                {
                    var entity = new UserGroup { UserId = userId, GroupId = groupId };
                    var groupExist = unitOfWork.GroupRepository.ExistById(groupId);
                    var userGroupExist = unitOfWork.UserGroupRepository.ExistById(userId, groupId);

                    if (groupExist && userGroupExist)
                    {
                        unitOfWork.UserGroupRepository.Delete(entity);
                    }
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Admin.User.UserGroupsUpdated);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            if(groupSensible != null)
                AddLogSensibleData(groupSensible, userExist);

            return response;
        }

        public Response<UserModel> GetUserInfo(int employeeId)
        {
            var response = new Response<UserModel>();

            var employee = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, employeeId);

            if (response.HasErrors()) return response;

            return SetUserInfo(employee.Email, response);
        }

        public ICollection<User> GetDirectors()
        {
            return unitOfWork.UserRepository.GetDirectors();
        }

        public bool HasGafGroup()
        {
            return unitOfWork.UserRepository.HasGafGroup(sessionManager.GetUserEmail());
        }

        public bool HasComplianceGroup()
        {
            return unitOfWork.UserRepository.HasComplianceGroup(sessionManager.GetUserEmail());
        }

        public Response<UserModel> GetUserInfo()
        {
            var email = sessionManager.GetUserEmail();

            var response = new Response<UserModel>();

            return SetUserInfo(email, response);
        }

        private Response<UserModel> SetUserInfo(string email, Response<UserModel> response)
        {
            var user = unitOfWork.UserRepository.GetSingle(x => x.Email.Equals(email));

            if (user == null)
            {
                response.AddError(Resources.Admin.User.NotFound);
                return response;
            }

            var model = Translate(user);

            var employee = unitOfWork.EmployeeRepository.GetUserInfo(email);

            model.EmployeeId = employee?.Id ?? 0;
            model.IsExternal = employee?.IsExternal ?? false;

            if (employee != null)
            {
                model.HasCreditCard = employee.HasCreditCard;
            }

            if (employee?.Manager != null)
            {
                model.ManagerId = employee.ManagerId.GetValueOrDefault();
                model.ManagerDesc = employee.Manager.Name;

                var authorizer = unitOfWork.UserApproverRepository.GetAuthorizerForLicenses(employee.Manager.UserName, employee.Id);

                if (authorizer != null)
                {
                    model.AuthorizerId = authorizer.Id;
                    model.AuthorizerDesc = authorizer.Name;
                }
            }

            var today = DateTime.UtcNow.Date;

            if (today.Day > 1) today = today.AddDays((today.Day - 1) * -1);

            var allocationFirst = employee?.Allocations.FirstOrDefault(x => x.StartDate.Date == today.Date);

            if (allocationFirst != null)
            {
                model.SectorId = allocationFirst.Analytic?.SectorId ?? 0;
                model.SectorDesc = allocationFirst.Analytic?.Sector?.Text;
            }

            response.Data = model;

            return response;
        }

        public bool HasDirectorGroup()
        {
            return unitOfWork.UserRepository.HasDirectorGroup(sessionManager.GetUserEmail());
        }

        public Response Add(User domain)
        {
            var response = new Response();

            try
            {
                unitOfWork.UserRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.User.Created);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public Response CheckIfExist(string mail)
        {
            var response = new Response();

            if (unitOfWork.UserRepository.ExistByMail(mail))
            {
                response.AddError(Resources.Admin.User.AlreadyExist);
            }

            return response;
        }

        public bool HasDafGroup()
        {
            return unitOfWork.UserRepository.HasDafGroup(sessionManager.GetUserEmail());
        }

        public bool HasCdgGroup()
        {
            return unitOfWork.UserRepository.HasCdgGroup(sessionManager.GetUserEmail());
        }

        public ICollection<User> GetManagers()
        {
            return unitOfWork.UserRepository.GetManagers();
        }

        public Response<List<UserSelectListItem>> GetCommercialManagers()
        {
            var result = unitOfWork.UserRepository.GetCommercialManagers();

            var response = new Response<List<UserSelectListItem>>
            {
                Data = result.Select(x =>
                        new UserSelectListItem
                        {
                            Id = x.Id.ToString(),
                            Text = x.Name,
                            ExternalId = x.ExternalManagerId,
                            UserName = x.UserName,
                            Email = x.Email
                        })
                    .OrderBy(x => x.Text)
                    .ToList()
            };

            return response;
        }

        public bool HasRrhhGroup()
        {
            return unitOfWork.UserRepository.HasRrhhGroup(sessionManager.GetUserEmail());
        }

        public bool HasManagerGroup()
        {
            return unitOfWork.UserRepository.HasManagersGroup(sessionManager.GetUserEmail());
        }

        public IList<User> GetAuthorizers()
        {
            return unitOfWork.UserRepository.GetAuthorizers();
        }

        public IList<User> GetExternalsFree()
        {
            return unitOfWork.UserRepository.GetExternalsFree();
        }

        private UserModel Translate(User user)
        {
            return mapper.Map<User, UserModel>(user);
        }
    }
}
