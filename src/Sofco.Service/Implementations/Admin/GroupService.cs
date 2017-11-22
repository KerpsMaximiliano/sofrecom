using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Framework.ValidationHelpers.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Admin
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository repository;

        private readonly IRoleRepository roleRepository;

        public GroupService(IGroupRepository repository, IRoleRepository roleRepository)
        {
            this.repository = repository;
            this.roleRepository = roleRepository;
        }

        public Response<Group> Active(int id, bool active)
        {
            var response = new Response<Group>();
            var entity = repository.GetSingle(x => x.Id == id);

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

                repository.Update(entity);
                repository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.Admin.Group.Enabled : Resources.Admin.Group.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Group> GetAllReadOnly(bool active)
        {
            if (active)
                return repository.GetAllActivesReadOnly();
            else
                return repository.GetAllReadOnly();
        }

        public Response<Group> GetById(int id)
        {
            var response = new Response<Group>();
            var group = repository.GetSingleFull(x => x.Id == id);

            if (group != null)
            {
                response.Data = group;
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Group.NotFound, MessageType.Error));
            return response;
        }

        public Response<Group> Insert(Group group)
        {
            var response = new Response<Group>();

            try
            {
                GroupValidationHelper.ValidateRol(group, response, roleRepository);
                GroupValidationHelper.ValidateDescription(group, response, repository);

                if (response.HasErrors()) return response;

                group.StartDate = DateTime.Now;

                repository.Insert(group);
                repository.Save();

                response.Data = group;
                response.Messages.Add(new Message(Resources.Admin.Group.Created, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
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
                        var role = roleRepository.GetSingle(x => x.Id == roleId);

                        if (role == null)
                        {
                            response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                        }
                             
                        group.Role = role;
                    }
                }

                if (response.HasErrors()) return response;

                GroupValidationHelper.ValidateDescription(group, response, repository);

                if (group.Active) group.EndDate = null;

                repository.Update(group);
                repository.Save();
                response.Messages.Add(new Message(Resources.Admin.Group.Updated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Group> DeleteById(int id)
        {
            var response = new Response<Group>();
            var entity = repository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.EndDate = DateTime.Now;

                repository.Delete(entity);
                repository.Save();

                response.Messages.Add(new Message(Resources.Admin.Group.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Group.NotFound, MessageType.Error));
            return response;
        }

        public Response<Group> AddRole(int roleId, int groupId)
        {
            var role = roleRepository.GetSingle(x => x.Id == roleId);

            var response = ValidateCommonRole(role, groupId);

            if (response.HasErrors()) return response;

            response.Data.Role = role;
            repository.Update(response.Data);
            repository.Save();

            response.Messages.Add(new Message(Resources.Admin.Group.RoleAssigned, MessageType.Success));
            return response;
        }

        public Response<Group> RemoveRole(int roleId, int groupId)
        {
            var role = roleRepository.GetSingle(x => x.Id == roleId);

            var response = ValidateCommonRole(role, groupId);

            if (response.HasErrors()) return response;

            response.Data.Role = null;
            repository.Update(response.Data);
            repository.Save();

            response.Messages.Add(new Message(Resources.Admin.Group.RoleRemoved, MessageType.Success));
            return response;
        }

        private Response<Group> ValidateCommonRole(Role role, int groupId)
        {
            var response = new Response<Group>();

            if (role == null)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            var group = repository.GetSingleWithRole(x => x.Id == groupId);

            if (group == null)
            {
                response.Messages.Add(new Message(Resources.Admin.Group.NotFound, MessageType.Error));
                return response;
            }

            response.Data = group;

            return response;
        }
    }
}
