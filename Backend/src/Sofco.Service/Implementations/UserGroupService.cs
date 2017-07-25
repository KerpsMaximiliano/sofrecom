using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using System.Collections.Generic;
using System;

namespace Sofco.Service.Implementations
{
    public class UserGroupService : IUserGroupService
    {
        IUserGroupRepository _repository;
        IRoleRepository _roleRepository;

        public UserGroupService(IUserGroupRepository repository, IRoleRepository roleRepository)
        {
            _repository = repository;
            _roleRepository = roleRepository;
        }

        public IList<UserGroup> GetAllReadOnly()
        {
            return _repository.GetAllReadOnly();
        }

        public Response<UserGroup> GetById(int id)
        {
            var response = new Response<UserGroup>();
            var userGroup = _repository.GetSingle(x => x.Id == id);

            if (userGroup != null)
            {
                response.Data = userGroup;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Group.NotFound, MessageType.Error));
            return response;
        }

        public Response<UserGroup> Insert(UserGroup userGroup)
        {
            var response = new Response<UserGroup>();

            try
            {
                if(userGroup.Role != null)
                {
                    var role = _roleRepository.GetSingle(x => x.Id == userGroup.Role.Id);

                    if(role == null)
                    {
                        response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                        return response;
                    }

                    userGroup.Role = role;
                }

                userGroup.StartDate = DateTime.Now;

                _repository.Insert(userGroup);
                _repository.Save(string.Empty);

                response.Data = userGroup;
                response.Messages.Add(new Message(Resources.es.Group.Created, MessageType.Success));
            }
            catch (Exception ex)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<UserGroup> Update(UserGroup userGroup)
        {
            var response = new Response<UserGroup>();
            var entity = _repository.GetSingle(x => x.Id == userGroup.Id);

            if (entity != null)
            {
                try
                {
                    if(userGroup.Role == null)
                    {
                        entity.Role = null;
                    }
                    else
                    {
                        if(userGroup.Role.Id != entity.Role.Id)
                        {
                            var role = _roleRepository.GetSingle(x => x.Id == userGroup.Role.Id);

                            if (role == null)
                            {
                                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                                return response;
                            }
                             
                            userGroup.Role = role;
                        }
                    }
                     
                    userGroup.ApplyTo(entity);

                    _repository.Update(entity);
                    _repository.Save(string.Empty);
                    response.Messages.Add(new Message(Resources.es.Group.Updated, MessageType.Success));
                }
                catch (Exception)
                {
                    response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
                }
            }
            else
            {
                response.Messages.Add(new Message(Resources.es.Group.NotFound, MessageType.Error));
            }

            return response;
        }

        public Response<UserGroup> DeleteById(int id)
        {
            var response = new Response<UserGroup>();
            var entity = _repository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.EndDate = DateTime.Now;

                _repository.Delete(entity);
                _repository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Group.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Group.NotFound, MessageType.Error));
            return response;
        }

        public Response<UserGroup> AddRole(int roleId, int userGroupId)
        {
            var response = new Response<UserGroup>();

            var role = _roleRepository.GetSingle(x => x.Id == roleId);

            if (role == null) {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                return response;
            }

            var userGroup = _repository.GetSingleWithRole(x => x.Id == userGroupId);

            if (userGroup == null)
            {
                response.Messages.Add(new Message(Resources.es.Group.NotFound, MessageType.Error));
                return response;
            }

            userGroup.Role = role;
            _repository.Update(userGroup);
            _repository.Save(string.Empty);

            response.Messages.Add(new Message(Resources.es.Group.RoleAssigned, MessageType.Success));
            return response;
        }
    }
}
