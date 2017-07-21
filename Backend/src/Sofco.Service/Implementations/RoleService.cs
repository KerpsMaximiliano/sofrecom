using System;
using System.Collections.Generic;
using Sofco.Core.Interfaces.DAL;
using Sofco.Core.Interfaces.Services;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Model.Enums;

namespace Sofco.Service.Implementations
{
    public class RoleService : IRoleService
    {
        IRoleRepository _repository;

        public RoleService(IRoleRepository repository)
        {
            _repository = repository;
        }

        public Response<Role> DeleteById(int id)
        {
            var response = new Response<Role>();
            var entity = _repository.GetSingle(x => x.Id == id);

            if(entity != null)
            {
                _repository.Delete(entity);
                _repository.Save(string.Empty);

                response.Messages.Add(new Message("Rol eliminado correctamente", MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message("Rol no encontrado", MessageType.Error));
            return response;
        }

        public IList<Role> GetAllReadOnly()
        {
            return _repository.GetAllReadOnly();
        }

        public Response<Role> GetById(int id)
        {
            var response = new Response<Role>();
            var role = _repository.GetSingle(x => x.Id == id);

            if(role != null)
            {
                response.Data = role;
                return response;
            }

            response.Messages.Add(new Message("Rol no encontrado", MessageType.Error));
            return response;
        }

        public Response<Role> Insert(Role role)
        {
            var response = new Response<Role>();

            try
            {
                _repository.Insert(role);
                _repository.Save(string.Empty);

                response.Data = role;
                response.Messages.Add(new Message("Rol creado exitosamente", MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message("Ocurrio un error al guardar", MessageType.Error));
            }

            return response;
        }

        public Response<Role> Update(Role role)
        {
            var response = new Response<Role>();
            var entity = _repository.Exist(role.Id);

            if (entity != null)
            {
                try
                {
                    _repository.Update(role);
                    _repository.Save(string.Empty);
                    response.Messages.Add(new Message("Rol modificado correctamente", MessageType.Success));
                }
                catch (Exception)
                {
                    response.Messages.Add(new Message("Ocurrio un error al guardar", MessageType.Error));
                }
            }
            else
            {
                response.Messages.Add(new Message("Rol no encontrado", MessageType.Error));
            }
           
            return response;
        }
    }
}
