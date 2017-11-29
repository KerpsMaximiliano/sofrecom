using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using Sofco.Core.Services.Admin;
using Sofco.DAL;
using Sofco.Model.Models.Admin;

namespace Sofco.Service.Implementations.Admin
{
    public class ModuleService : IModuleService
    {
        private readonly IUnitOfWork unitOfWork;

        public ModuleService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Response<Module> Active(int id, bool active)
        {
            var response = new Response<Module>();
            var entity = unitOfWork.ModuleRepository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                entity.Active = active;

                unitOfWork.ModuleRepository.Update(entity);
                unitOfWork.Save();

                response.Data = entity;
                response.Messages.Add(new Message(active ? Resources.Admin.Module.Enabled : Resources.Admin.Module.Disabled, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Module.NotFound, MessageType.Error));
            return response;
        }

        public IList<Module> GetAllReadOnly(bool active)
        {
            if (active)
                return unitOfWork.ModuleRepository.GetAllActivesReadOnly();
            else
                return unitOfWork.ModuleRepository.GetAllReadOnly();
        }

        public Response<Module> GetById(int id)
        {
            var response = new Response<Module>();
            var entity = unitOfWork.ModuleRepository.GetSingleWithFunctionalities(x => x.Id == id);

            if (entity != null)
            {
                response.Data = entity;
                return response;
            }

            response.Messages.Add(new Message(Resources.Admin.Module.NotFound, MessageType.Error));
            return response;
        }

        public Response<Module> Update(Module data)
        {
            var response = new Response<Module>();

            try
            {
                unitOfWork.ModuleRepository.Update(data);
                unitOfWork.Save();
                response.Messages.Add(new Message(Resources.Admin.Module.Updated, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Module> GetAllWithFunctionalitiesReadOnly()
        {
            return unitOfWork.ModuleRepository.GetAllWithFunctionalitiesReadOnly();
        }
    }
}
