using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Admin;
using Sofco.Framework.ValidationHelpers.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Admin
{
    public class GroupService : IGroupService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<GroupService> logger;

        public GroupService(IUnitOfWork unitOfWork, ILogMailer<GroupService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response<Group> Active(int id, bool active)
        {
            var response = new Response<Group>();
            var entity = unitOfWork.GroupRepository.GetSingle(x => x.Id == id);

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

                unitOfWork.GroupRepository.Update(entity);
                unitOfWork.Save();

                response.Data = entity;
                response.AddSuccess(active ? Resources.Admin.Group.Enabled : Resources.Admin.Group.Disabled);
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Functionality.NotFound, MessageType.Error));
            return response;
        }

        public IList<Group> GetAllReadOnly(bool active)
        {
            if (active)
                return unitOfWork.GroupRepository.GetAllActivesReadOnly();
            else
                return unitOfWork.GroupRepository.GetAllReadOnly();
        }

        public Response<Group> GetById(int id)
        {
            var response = new Response<Group>();
            var group = unitOfWork.GroupRepository.GetSingleFull(x => x.Id == id);

            if (group != null)
            {
                response.Data = group;
                return response;
            }

            response.AddError(Resources.Admin.Group.NotFound);
            return response;
        }

        public Response<Group> Insert(Group group)
        {
            var response = new Response<Group>();

            try
            {
                GroupValidationHelper.ValidateMail(group, response);
                GroupValidationHelper.ValidateRol(group, response, unitOfWork.RoleRepository);
                GroupValidationHelper.ValidateDescription(group, response, unitOfWork.GroupRepository);

                if (response.HasErrors()) return response;

                group.StartDate = DateTime.Now;

                unitOfWork.GroupRepository.Insert(group);
                unitOfWork.Save();

                response.Data = group;
                response.AddSuccess(Resources.Admin.Group.Created);
            }
            catch(Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
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
                        var role = unitOfWork.RoleRepository.GetSingle(x => x.Id == roleId);

                        if (role == null)
                        {
                            response.AddError(Resources.Admin.Role.NotFound);
                        }
                             
                        group.Role = role;
                    }
                }

                if (response.HasErrors()) return response;

                GroupValidationHelper.ValidateDescription(group, response, unitOfWork.GroupRepository);

                if (group.Active) group.EndDate = null;

                unitOfWork.GroupRepository.Update(group);
                unitOfWork.Save();
                response.AddSuccess(Resources.Admin.Group.Updated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<Group> DeleteById(int id)
        {
            var response = new Response<Group>();
            var entity = unitOfWork.GroupRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.EndDate = DateTime.Now;

                unitOfWork.GroupRepository.Delete(entity);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Group.Deleted);
                return response;
            }

            response.AddError(Resources.Admin.Group.NotFound);
            return response;
        }

        public Response<Group> AddRole(int roleId, int groupId)
        {
            var role = unitOfWork.RoleRepository.GetSingle(x => x.Id == roleId);

            var response = ValidateCommonRole(role, groupId);

            if (response.HasErrors()) return response;

            try
            {
                response.Data.Role = role;
                unitOfWork.GroupRepository.Update(response.Data);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Group.RoleAssigned);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<Group> RemoveRole(int roleId, int groupId)
        {
            var role = unitOfWork.RoleRepository.GetSingle(x => x.Id == roleId);

            var response = ValidateCommonRole(role, groupId);

            if (response.HasErrors()) return response;

            try
            {
                response.Data.Role = null;
                unitOfWork.GroupRepository.Update(response.Data);
                unitOfWork.Save();

                response.AddSuccess(Resources.Admin.Group.RoleRemoved);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }
   
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

            var group = unitOfWork.GroupRepository.GetSingleWithRole(x => x.Id == groupId);

            if (group == null)
            {
                response.AddError(Resources.Admin.Group.NotFound);
                return response;
            }

            response.Data = group;

            return response;
        }
    }
}
