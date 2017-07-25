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

            response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
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
                response.Messages.Add(new Message(Resources.es.Role.Created, MessageType.Success));
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Role> Update(Role role)
        {
            var response = new Response<Role>();
            var entity = _repository.GetSingle(x => x.Id == role.Id);

            if (entity != null)
            {
                try
                {
                    _repository.Update(role);
                    _repository.Save(string.Empty);
                    response.Messages.Add(new Message(Resources.es.Role.Updated, MessageType.Success));
                }
                catch (Exception)
                {
                    response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
                }
            }
            else
            {
                response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
            }
           
            return response;
        }

        public Response<Role> DeleteById(int id)
        {
            var response = new Response<Role>();
            var entity = _repository.GetSingle(x => x.Id == id);

            if (entity != null)
            {
                _repository.Delete(entity);
                _repository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Role.Deleted, MessageType.Success));
                return response;
            }

            response.Messages.Add(new Message(Resources.es.Role.NotFound, MessageType.Error));
            return response;
        }
    }
}
