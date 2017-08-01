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


        public Response<Group> Active(int id, bool active)
        {
            var response = new Response<Group>();
            var entity = _repository.GetSingle(x => x.Id == id);

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

                _repository.Update(entity);
                _repository.Save(string.Empty);

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Group.Enabled : Resources.es.Group.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Functionality.NotFound, MessageType.Error));
            return response;
        }


        public IList<Group> GetAllFullReadOnly()
        {
            return _repository.GetAllFullReadOnly();
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

        public Response<Group> Update(Group group, int roleId)
        {
            var response = new Response<Group>();

            try
            {
                if(roleId == 0)
                {
                    group.Role = null;
                }
                else
                {
                    if(group.Role == null || roleId != group.Role.Id)
                    {
                        var role = _roleRepository.GetSingle(x => x.Id == roleId);

                        if (role == null)
                        {
                            response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
                            return response;
                        }
                             
                        group.Role = role;
                    }
                }
                     
                group.ApplyTo(group);

                if (group.Active) group.EndDate = null;

                _repository.Update(group);
                _repository.Save(string.Empty);
                response.Messages.Add(new Message(Resources.es.Group.Updated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
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
