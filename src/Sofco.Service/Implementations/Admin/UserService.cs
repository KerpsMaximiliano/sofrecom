using System;
using System.Collections.Generic;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Model.Utils;
using Sofco.Core.Services.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations.Admin
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<UserService> logger;
        private readonly ISessionManager sessionManager;

        public UserService(IUnitOfWork unitOfWork, ILogMailer<UserService> logger, ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.sessionManager = sessionManager;
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

            var userGroup = unitOfWork.GroupRepository.GetSingle(x => x.Id == groupId);

            if (userGroup == null)
            {
                response.AddError(Resources.Admin.Group.NotFound);
                return response;
            }

            var entity = new UserGroup { UserId = userId, GroupId = groupId };

            unitOfWork.UserGroupRepository.Insert(entity);
            unitOfWork.Save();

            response.AddSuccess(Resources.Admin.User.GroupAssigned);

            return response;
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

            var userExist = unitOfWork.UserRepository.ExistById(userId);

            if (!userExist)
            {
                response.AddError(Resources.Admin.User.NotFound);
                return response;
            }

            try
            {
                foreach (var groupId in groupsToAdd)
                {
                    var entity = new UserGroup { UserId = userId, GroupId = groupId };
                    var groupExist = unitOfWork.GroupRepository.ExistById(groupId);
                    var userGroupExist = unitOfWork.UserGroupRepository.ExistById(userId, groupId);

                    if (groupExist && !userGroupExist)
                    {
                        unitOfWork.UserGroupRepository.Insert(entity);
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

            return response;
        }

        public Response<User> GetByMail()
        {
            var email = sessionManager.GetUserMail();

            var response = new Response<User>();

            var user = unitOfWork.UserRepository.GetSingle(x => x.Email.Equals(email));

            if(user == null)
            {
                response.AddError(Resources.Admin.User.NotFound);
                return response;
            }

            response.Data = user;
            return response;
        }

        public bool HasDirectorGroup()
        {
            return unitOfWork.UserRepository.HasDirectorGroup(sessionManager.GetUserMail());
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
            return unitOfWork.UserRepository.HasDafGroup(sessionManager.GetUserMail());
        }

        public bool HasCdgGroup()
        {
            return unitOfWork.UserRepository.HasDafGroup(sessionManager.GetUserMail());
        }

        public ICollection<User> GetManagers()
        {
            return unitOfWork.UserRepository.GetManagers();
        }
    }
}
