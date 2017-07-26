using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using System.Collections.Generic;
using System;

namespace Sofco.Service.Implementations
{
    public class GroupService : IGroupService
    {
        IGroupRepository _repository;
        IRoleRepository _roleRepository;

        public GroupService(IGroupRepository repository, IRoleRepository roleRepository)
        {
            _repository = repository;
            _roleRepository = roleRepository;
        }

        public IList<Group> GetAllReadOnly()
        {
            return _repository.GetAllReadOnly();
        }

        public Response<Group> GetById(int id)
        {
            var response = new Response<Group>();
            var group = _repository.GetSingleFull(x => x.Id == id);

            if (group != null)
            {
                response.Data = group;
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Group.NotFound, MessageType.Error));
            return response;
        }

        public Response<Group> Insert(Group group)
        {
            var response = new Response<Group>();

            try
            {
                if(group.Role != null)
                {
                    var role = _roleRepository.GetSingle(x => x.Id == group.Role.Id);

                    if(role == null)
                    {
                        response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                        return response;
                    }

                    group.Role = role;
                }

                group.StartDate = DateTime.Now;

                _repository.Insert(group);
                _repository.Save(string.Empty);

                response.Data = group;
                response.Messages.Add(new Message(Resources.es.Group.Created, MessageType.Success));
            }
            catch (Exception ex)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Group> Update(Group group)
        {
            var response = new Response<Group>();
            var entity = _repository.GetSingle(x => x.Id == group.Id);

            if (entity != null)
            {
                try
                {
                    if(group.Role == null)
                    {
                        entity.Role = null;
                    }
                    else
                    {
                        if(group.Role.Id != entity.Role.Id)
                        {
                            var role = _roleRepository.GetSingle(x => x.Id == group.Role.Id);

                            if (role == null)
                            {
                                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                                return response;
                            }
                             
                            group.Role = role;
                        }
                    }
                     
                    group.ApplyTo(entity);

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

        public Response<Group> DeleteById(int id)
        {
            var response = new Response<Group>();
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

        public Response<Group> AddRole(int roleId, int groupId)
        {
            var role = _roleRepository.GetSingle(x => x.Id == roleId);

            var response = ValidateCommonRole(role, groupId);

            if (response.HasErrors()) return response;

            response.Data.Role = role;
            _repository.Update(response.Data);
            _repository.Save(string.Empty);

            response.Messages.Add(new Message(Resources.es.Group.RoleAssigned, MessageType.Success));
            return response;
        }

        public Response<Group> RemoveRole(int roleId, int groupId)
        {
            var role = _roleRepository.GetSingle(x => x.Id == roleId);

            var response = ValidateCommonRole(role, groupId);

            if (response.HasErrors()) return response;

            response.Data.Role = null;
            _repository.Update(response.Data);
            _repository.Save(string.Empty);

            response.Messages.Add(new Message(Resources.es.Group.RoleRemoved, MessageType.Success));
            return response;
        }

        private Response<Group> ValidateCommonRole(Role role, int groupId)
        {
            var response = new Response<Group>();

            if (role == null)
            {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                return response;
            }

            var group = _repository.GetSingleWithRole(x => x.Id == groupId);

            if (group == null)
            {
                response.Messages.Add(new Message(Resources.es.Group.NotFound, MessageType.Error));
                return response;
            }

            response.Data = group;

            return response;
        }
    }
}
