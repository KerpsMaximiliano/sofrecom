using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
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

        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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
                response.Messages.Add(new Message(active ? Resources.Admin.User.Enabled : Resources.Admin.User.Disabled, MessageType.Success));
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
                response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
                return response;
            }

            var userGroup = unitOfWork.GroupRepository.GetSingle(x => x.Id == groupId);

            if (userGroup == null)
            {
                response.Messages.Add(new Message(Resources.Admin.Group.NotFound, MessageType.Error));
                return response;
            }

            var entity = new UserGroup { UserId = userId, GroupId = groupId };

            unitOfWork.UserGroupRepository.Insert(entity);
            unitOfWork.Save();

            response.Messages.Add(new Message(Resources.Admin.User.GroupAssigned, MessageType.Success));

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

            response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
            return response;
        }

        public Response<User> RemoveUserGroup(int userId, int groupId)
        {
            var response = new Response<User>();

            var userExist = unitOfWork.UserRepository.ExistById(userId);

            if (!userExist)
            {
                response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
                return response;
            }

            var groupExist = unitOfWork.GroupRepository.ExistById(groupId);

            if (!groupExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Group.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                var entity = new UserGroup { UserId = userId, GroupId = groupId };

                unitOfWork.UserGroupRepository.Delete(entity);
                unitOfWork.Save();

                response.Messages.Add(new Message(Resources.Admin.User.GroupRemoved, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<User> ChangeUserGroups(int userId, List<int> groupsToAdd, List<int> groupsToRemove)
        {
            var response = new Response<User>();

            var userExist = unitOfWork.UserRepository.ExistById(userId);

            if (!userExist)
            {
                response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
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
                response.Messages.Add(new Message(Resources.Admin.User.UserGroupsUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<User> GetByMail(string mail)
        {
            var response = new Response<User>();

            var user = unitOfWork.UserRepository.GetSingle(x => x.Email.Equals(mail));

            if(user == null)
            {
                response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
                return response;
            }

            response.Data = user;
            return response;
        }

        public bool HasDirectorGroup(string userMail)
        {
            return unitOfWork.UserRepository.HasDirectorGroup(userMail);
        }

        public Response Add(User domain)
        {
            var response = new Response();

            try
            {
                unitOfWork.UserRepository.Insert(domain);
                unitOfWork.Save();

                response.Messages.Add(new Message(Resources.Admin.User.Created, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response CheckIfExist(string mail)
        {
            var response = new Response();

            if (unitOfWork.UserRepository.ExistByMail(mail))
            {
                response.Messages.Add(new Message(Resources.Admin.User.AlreadyExist, MessageType.Error));
            }

            return response;
        }

        public bool HasDafGroup(string userMail, string dafCode)
        {
            return unitOfWork.UserRepository.HasDafGroup(userMail, dafCode);
        }

        public bool HasCdgGroup(string userMail, string cdgCode)
        {
            return unitOfWork.UserRepository.HasDafGroup(userMail, cdgCode);
        }
    }
}
