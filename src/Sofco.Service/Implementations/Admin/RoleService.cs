using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using Sofco.Model.Relationships;
using Sofco.Core.Services.Admin;
using Sofco.DAL;
using Sofco.Framework.ValidationHelpers.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Response<Role> Active(int id, bool active)
        {
            var response = new Response<Role>();
            var entity = unitOfWork.RoleRepository.GetSingle(x => x.Id == id);

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

                unitOfWork.RoleRepository.Update(entity);
                unitOfWork.Save();

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
                return unitOfWork.RoleRepository.GetAllActivesReadOnly();
            else
                return unitOfWork.RoleRepository.GetAllReadOnly();
        }

        public Response<Role> GetDetail(int id)
        {
            var response = new Response<Role>();
            var role = unitOfWork.RoleRepository.GetDetail(id);

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
            var role = unitOfWork.RoleRepository.GetSingle(x => x.Id == id);

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

            RoleValidationHelper.ValidateIfDescriptionExist(response, unitOfWork.RoleRepository, role);

            if (response.HasErrors()) return response;

            try
            {
                role.StartDate = DateTime.Now;

                unitOfWork.RoleRepository.Insert(role);
                unitOfWork.RoleRepository.Save();

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

            RoleValidationHelper.ValidateIfDescriptionExist(response, unitOfWork.RoleRepository, role);

            if (response.HasErrors()) return response;

            try
            {
                if (role.Active) role.EndDate = null;

                unitOfWork.RoleRepository.Update(role);
                unitOfWork.Save();
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
            var entity = unitOfWork.RoleRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.EndDate = DateTime.Now;

                unitOfWork.RoleRepository.Delete(entity);
                unitOfWork.Save();

                response.Data = entity;
                response.Messages.Add(new Message(Resources.Admin.Role.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
            return response;
        }

        public IList<Role> GetRolesByGroup(IEnumerable<int> groupIds)
        {
            return unitOfWork.RoleRepository.GetRolesByGroup(groupIds);
        }

        public Response<Role> AddFunctionalities(int roleId, List<int> functionalitiesToAdd)
        {
            var response = new Response<Role>();

            var roleExist = unitOfWork.RoleRepository.ExistById(roleId);

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
                    var functionalityExist = unitOfWork.FunctionalityRepository.ExistById(functionalityId);
                    var roleFunctionalityExist = unitOfWork.RoleFunctionalityRepository.ExistById(roleId, functionalityId);

                    if (functionalityExist && !roleFunctionalityExist)
                    {
                        unitOfWork.RoleFunctionalityRepository.Insert(entity);
                    }
                }

                unitOfWork.Save();
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

            var roleExist = unitOfWork.RoleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            var functionalityExist = unitOfWork.FunctionalityRepository.ExistById(functionalityId);

            if (!functionalityExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Module.NotFound, MessageType.Error));
                return response;
            }

            var rolefunctionalityExist = unitOfWork.RoleFunctionalityRepository.ExistById(roleId, functionalityId);

            if (rolefunctionalityExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.RoleModuleAlreadyCreated, MessageType.Error));
            }
            else
            {
                var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                unitOfWork.RoleFunctionalityRepository.Insert(entity);
                unitOfWork.Save();
                response.Messages.Add(new Message(Resources.Admin.Role.ModulesUpdated, MessageType.Success));
            }

            return response;
        }

        public Response<Role> DeleteFunctionality(int roleId, int functionalityId)
        {
            var response = new Response<Role>();

            var roleExist = unitOfWork.RoleRepository.ExistById(roleId);

            if (!roleExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                return response;
            }

            var functionalityExist = unitOfWork.FunctionalityRepository.ExistById(functionalityId);

            if (!functionalityExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Module.NotFound, MessageType.Error));
                return response;
            }

            var rolefunctionalityExist = unitOfWork.RoleFunctionalityRepository.ExistById(roleId, functionalityId);

            if (!rolefunctionalityExist)
            {
                response.Messages.Add(new Message(Resources.Admin.Role.RoleModuleAlreadyRemoved, MessageType.Error));
            }
            else
            {
                var entity = new RoleFunctionality { RoleId = roleId, FunctionalityId = functionalityId };
                unitOfWork.RoleFunctionalityRepository.Delete(entity);
                unitOfWork.Save();
                response.Messages.Add(new Message(Resources.Admin.Role.ModulesUpdated, MessageType.Success));
            }

            return response;
        }

        public Response<Role> RemoveFunctionalities(int roleId, List<int> functionalities)
        {
            var response = new Response<Role>();

            var roleExist = unitOfWork.RoleRepository.ExistById(roleId);

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
                    var functionalityExist = unitOfWork.FunctionalityRepository.ExistById(functionalityId);
                    var roleFunctionalityExist = unitOfWork.RoleFunctionalityRepository.ExistById(roleId, functionalityId);

                    if (functionalityExist && roleFunctionalityExist)
                    {
                        unitOfWork.RoleFunctionalityRepository.Delete(entity);
                    }
                }

                unitOfWork.Save();
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
