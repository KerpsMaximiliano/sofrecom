using System;
using System.Collections.Generic;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using Sofco.Core.DAL.Admin;
using Sofco.Model.Relationships;
using Sofco.Core.Services.Admin;
using Sofco.Framework.ValidationHelpers.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;

        private readonly IRoleFunctionalityRepository roleFunctionalityRepository;

        private readonly IFunctionalityRepository functionalityRepository;

        public RoleService(IRoleRepository repository, IRoleFunctionalityRepository roleFunctionality, IFunctionalityRepository functionalityRepository)
        {
            roleRepository = repository;
            roleFunctionalityRepository = roleFunctionality;
            this.functionalityRepository = functionalityRepository;
        }

        public Response<Role> Active(int id, bool active)
        {
            var response = new Response<Role>();
            var entity = roleRepository.GetSingle(x => x.Id == id);

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

                roleRepository.Update(entity);
                roleRepository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.Admin.Role.Enabled : Resources.Admin.Role.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Role> GetAllReadOnly(bool active)
        {
            if (active)
                return roleRepository.GetAllActivesReadOnly();
            else
                return roleRepository.GetAllReadOnly();
        }

        public Response<Role> GetDetail(int id)
        {
            var response = new Response<Role>();
            var role = roleRepository.GetDetail(id);

            if (role != null)
            {
                response.Data = role;
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
            return response;
        }

        public Response<Role> GetById(int id)
        {
            var response = new Response<Role>();
            var role = roleRepository.GetSingle(x => x.Id == id);

            if(role != null)
            {
                response.Data = role;
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
            return response;
        }

        public Response<Role> Insert(Role role)
        {
            var response = new Response<Role>();

            RoleValidationHelper.ValidateIfDescriptionExist(response, roleRepository, role);

            if (response.HasErrors()) return response;

            try
            {
                role.StartDate = DateTime.Now;

                roleRepository.Insert(role);
                roleRepository.Save();

                response.Data = role;
                response.Messages.Add(new Message(Resources.Admin.Role.Created, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Role> Update(Role role)
        {
            var response = new Response<Role>();

            RoleValidationHelper.ValidateIfDescriptionExist(response, roleRepository, role);

            if (response.HasErrors()) return response;

            try
            {
                if (role.Active) role.EndDate = null;

                roleRepository.Update(role);
                roleRepository.Save();
                response.Messages.Add(new Message(Resources.Admin.Role.Updated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }
           
            return response;
        }

        public Response<Role> DeleteById(int id)
        {
            var response = new Response<Role>();
            var entity = roleRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.EndDate = DateTime.Now;

                roleRepository.Delete(entity);
                roleRepository.Save();

                response.Data = entity;
                response.Messages.Add(new Message(Resources.Admin.Role.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
            return response;
        }

        public IList<Role> GetRolesByGroup(IEnumerable<int> groupIds)
        {
            return roleRepository.GetRolesByGroup(groupIds);
        }

        public Response<Role> AddFunctionalities(int roleId, List<int> functionalitiesToAdd)
        {
            var response = new Response<Role>();

            var roleExist = roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var functionalityId in functionalitiesToAdd)
                {
                    var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                    var functionalityExist = functionalityRepository.ExistById(functionalityId);
                    var roleFunctionalityExist = roleFunctionalityRepository.ExistById(roleId, functionalityId);

                    if (functionalityExist && !roleFunctionalityExist)
                    {
                        roleFunctionalityRepository.Insert(entity);
                    }
                }

                roleFunctionalityRepository.Save();
                response.Messages.Add(new Message(Resources.Admin.Role.ModulesUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Role> AddFunctionality(int roleId, int functionalityId)
        {
            var response = new Response<Role>();

            var roleExist = roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            var functionalityExist = functionalityRepository.ExistById(functionalityId);

            if (!functionalityExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Module.NotFound, MessageType.Error));
                return response;
            }

            var rolefunctionalityExist = roleFunctionalityRepository.ExistById(roleId, functionalityId);

            if (rolefunctionalityExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.RoleModuleAlreadyCreated, MessageType.Error));
            }
            else
            {
                var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                roleFunctionalityRepository.Insert(entity);
                roleFunctionalityRepository.Save();
                response.Messages.Add(new Message(Resources.Admin.Role.ModulesUpdated, MessageType.Success));
            }

            return response;
        }

        public Response<Role> DeleteFunctionality(int roleId, int functionalityId)
        {
            var response = new Response<Role>();

            var roleExist = roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            var functionalityExist = functionalityRepository.ExistById(functionalityId);

            if (!functionalityExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Module.NotFound, MessageType.Error));
                return response;
            }

            var rolefunctionalityExist = roleFunctionalityRepository.ExistById(roleId, functionalityId);

            if (!rolefunctionalityExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.RoleModuleAlreadyRemoved, MessageType.Error));
            }
            else
            {
                var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                roleFunctionalityRepository.Delete(entity);
                roleFunctionalityRepository.Save();
                response.Messages.Add(new Message(Resources.Admin.Role.ModulesUpdated, MessageType.Success));
            }

            return response;
        }

        public Response<Role> RemoveFunctionalities(int roleId, List<int> functionalities)
        {
            var response = new Response<Role>();

            var roleExist = roleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            try
            {
                foreach (var functionalityId in functionalities)
                {
                    var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                    var functionalityExist = functionalityRepository.ExistById(functionalityId);
                    var roleFunctionalityExist = roleFunctionalityRepository.ExistById(roleId, functionalityId);

                    if (functionalityExist && roleFunctionalityExist)
                    {
                        roleFunctionalityRepository.Delete(entity);
                    }
                }

                roleFunctionalityRepository.Save();
                response.Messages.Add(new Message(Resources.Admin.Role.ModulesUpdated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }
    }
}
