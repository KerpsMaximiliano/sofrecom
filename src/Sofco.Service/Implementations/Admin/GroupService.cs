using Sofco.Model.Utils;
using Sofco.Model.Enums;
using System.Collections.Generic;
using System;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Framework.ValidationHelpers.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _repository;
        private readonly IRoleRepository _roleRepository;

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
                _repository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.es.Admin.Group.Enabled : Resources.es.Admin.Group.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Group> GetAllReadOnly(bool active)
        {
            if (active)
                return _repository.GetAllActivesReadOnly();
            else
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

            response.Messages.Add(new Message(Resources.es.Admin.Group.NotFound, MessageType.Error));
            return response;
        }

        public Response<Group> Insert(Group group)
        {
            var response = new Response<Group>();

            try
            {
                GroupValidationHelper.ValidateRol(group, response, _roleRepository);
                GroupValidationHelper.ValidateDescription(group, response, _repository);

                if (response.HasErrors()) return response;

                group.StartDate = DateTime.Now;

                _repository.Insert(group);
                _repository.Save();

                response.Data = group;
                response.Messages.Add(new Message(Resources.es.Admin.Group.Created, MessageType.Success));
            }
            catch
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
                            response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
                        }
                             
                        group.Role = role;
                    }
                }

                if (response.HasErrors()) return response;

                GroupValidationHelper.ValidateDescription(group, response, _repository);

                if (group.Active) group.EndDate = null;

                _repository.Update(group);
                _repository.Save();
                response.Messages.Add(new Message(Resources.es.Admin.Group.Updated, MessageType.Success));
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
                _repository.Save();

                response.Messages.Add(new Message(Resources.es.Admin.Group.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Admin.Group.NotFound, MessageType.Error));
            return response;
        }

        public Response<Group> AddRole(int roleId, int groupId)
        {
            var role = _roleRepository.GetSingle(x => x.Id == roleId);

            var response = ValidateCommonRole(role, groupId);

            if (response.HasErrors()) return response;

            response.Data.Role = role;
            _repository.Update(response.Data);
            _repository.Save();

            response.Messages.Add(new Message(Resources.es.Admin.Group.RoleAssigned, MessageType.Success));
            return response;
        }

        public Response<Group> RemoveRole(int roleId, int groupId)
        {
            var role = _roleRepository.GetSingle(x => x.Id == roleId);

            var response = ValidateCommonRole(role, groupId);

            if (response.HasErrors()) return response;

            response.Data.Role = null;
            _repository.Update(response.Data);
            _repository.Save();

            response.Messages.Add(new Message(Resources.es.Admin.Group.RoleRemoved, MessageType.Success));
            return response;
        }

        private Response<Group> ValidateCommonRole(Role role, int groupId)
        {
            var response = new Response<Group>();

            if (role == null)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            var group = _repository.GetSingleWithRole(x => x.Id == groupId);

            if (group == null)
            {
                response.Messages.Add(new Message(Resources.es.Admin.Group.NotFound, MessageType.Error));
                return response;
            }

            response.Data = group;

            return response;
        }
    }
}
