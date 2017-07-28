using Sofco.Core.Services;
using System;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Core.DAL;
using Sofco.Model.Enums;
using Sofco.Core.Interfaces.DAL;
using System.Linq;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations
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
                _userRepository.Save(string.Empty);

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.User.Enabled : Resources.es.User.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.User.NotFound, MessageType.Error));
            return response;
        }

        public Response<User> AddUserGroup(int userId, int groupId)
        {
            var response = new Response<User>();

            var user = _userRepository.GetSingleWithUserGroup(x => x.Id == userId);

            if (user == null)
            {
                response.Messages.Add(new Message(Resources.es.User.NotFound, MessageType.Error));
                return response;
            }

            var userGroup = _groupRepository.GetSingle(x => x.Id == groupId);

            if (userGroup == null)
            {
                response.Messages.Add(new Message(Resources.es.Group.NotFound, MessageType.Error));
                return response;
            }

            var entity = new UserGroup { UserId = userId, GroupId = groupId };

            _userGroupRepository.Insert(entity);
            _userGroupRepository.Save(string.Empty);

            response.Messages.Add(new Message(Resources.es.User.GroupAssigned, MessageType.Success));

            return response;
        }

        public IList<User> GetAllFullReadOnly()
        {
            return _userRepository.GetAllFullReadOnly();
        }

        public IList<User> GetAllReadOnly()
        {
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

            response.Messages.Add(new Message(Resources.es.User.NotFound, MessageType.Error));
            return response;
        }

        public Response<User> RemoveUserGroup(int userId, int groupId)
        {
            var response = new Response<User>();

            var userExist = _userRepository.ExistById(userId);

            if (!userExist)
            {
                response.Messages.Add(new Message(Resources.es.User.NotFound, MessageType.Error));
                return response;
            }

            var groupExist = _groupRepository.ExistById(groupId);

            if (!groupExist)
            {
                response.Messages.Add(new Message(Resources.es.Group.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                var entity = new UserGroup { UserId = userId, GroupId = groupId };

                _userGroupRepository.Delete(entity);
                _userGroupRepository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.User.GroupRemoved, MessageType.Success));
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
                response.Messages.Add(new Message(Resources.es.User.NotFound, MessageType.Error));
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

                _userGroupRepository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.User.UserGroupsUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }
    }
}
