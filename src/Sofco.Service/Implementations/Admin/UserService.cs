using System;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Relationships;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public UserService(IUserRepository userRepository, IGroupRepository groupRepository, IUserGroupRepository userGroupRepository)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
        }

        public Response<User> Active(int id, bool active)
        {
            var response = new Response<User>();
            var entity = _userRepository.GetSingle(x => x.Id == id);

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

                _userRepository.Update(entity);
                _userRepository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Admin.User.Enabled : Resources.es.Admin.User.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
            return response;
        }

        public Response<User> AddUserGroup(int userId, int groupId)
        {
            var response = new Response<User>();

            var user = _userRepository.GetSingleWithUserGroup(x => x.Id == userId);

            if (user == null)
            {
                response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
                return response;
            }

            var userGroup = _groupRepository.GetSingle(x => x.Id == groupId);

            if (userGroup == null)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Group.NotFound, MessageType.Error));
                return response;
            }

            var entity = new UserGroup { UserId = userId, GroupId = groupId };

            _userGroupRepository.Insert(entity);
            _userGroupRepository.Save();

            response.Messages.Add(new Message(Resources.es.Admin.User.GroupAssigned, MessageType.Success));

            return response;
        }

        public IList<User> GetAllReadOnly(bool active)
        {
            if (active)
                return _userRepository.GetAllActivesReadOnly();
            else
                return _userRepository.GetAllReadOnly();
        }

        public Response<User> GetById(int id)
        {
            var response = new Response<User>();
            var entity = _userRepository.GetSingleWithUserGroup(x => x.Id == id);

            if (entity != null)
            {
                response.Data = entity;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
            return response;
        }

        public Response<User> RemoveUserGroup(int userId, int groupId)
        {
            var response = new Response<User>();

            var userExist = _userRepository.ExistById(userId);

            if (!userExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
                return response;
            }

            var groupExist = _groupRepository.ExistById(groupId);

            if (!groupExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Group.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                var entity = new UserGroup { UserId = userId, GroupId = groupId };

                _userGroupRepository.Delete(entity);
                _userGroupRepository.Save();

                response.Messages.Add(new Message(Resources.es.Admin.User.GroupRemoved, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<User> ChangeUserGroups(int userId, List<int> groupsToAdd, List<int> groupsToRemove)
        {
            var response = new Response<User>();

            var userExist = _userRepository.ExistById(userId);

            if (!userExist)
            {
                response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var groupId in groupsToAdd)
                {
                    var entity = new UserGroup { UserId = userId, GroupId = groupId };
                    var groupExist = _groupRepository.ExistById(groupId);
                    var userGroupExist = _userGroupRepository.ExistById(userId, groupId);

                    if (groupExist && !userGroupExist)
                    {
                        _userGroupRepository.Insert(entity);
                    }
                }

                foreach (var groupId in groupsToRemove)
                {
                    var entity = new UserGroup { UserId = userId, GroupId = groupId };
                    var groupExist = _groupRepository.ExistById(groupId);
                    var userGroupExist = _userGroupRepository.ExistById(userId, groupId);

                    if (groupExist && userGroupExist)
                    {
                        _userGroupRepository.Delete(entity);
                    }
                }

                _userGroupRepository.Save();
                response.Messages.Add(new Message(Resources.es.Admin.User.UserGroupsUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<User> GetByMail(string mail)
        {
            var response = new Response<User>();

            var user = _userRepository.GetSingle(x => x.Email.Equals(mail));

            if(user == null)
            {
                response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
                return response;
            }

            response.Data = user;
            return response;
        }

        public bool HasDirectorGroup(string userMail)
        {
            return _userRepository.HasDirectorGroup(userMail);
        }

        public Response Add(User domain)
        {
            var response = new Response();

            try
            {
                _userRepository.Insert(domain);
                _userRepository.Save();

                response.Messages.Add(new Message(Resources.es.Admin.User.Created, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response CheckIfExist(string mail)
        {
            var response = new Response();

            if (_userRepository.ExistByMail(mail))
            {
                response.Messages.Add(new Message(Resources.es.Admin.User.AlreadyExist, MessageType.Error));
            }

            return response;
        }

        public bool HasDafGroup(string userMail, int dafMail)
        {
            return _userRepository.HasDafGroup(userMail, dafMail);
        }

        public bool HasCdgGroup(string userMail, int cdgMail)
        {
            return _userRepository.HasDafGroup(userMail, cdgMail);
        }
    }
}
