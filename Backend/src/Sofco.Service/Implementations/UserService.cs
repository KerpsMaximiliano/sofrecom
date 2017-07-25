using Sofco.Core.Services;
using System;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Utils;
using Sofco.Core.DAL;

namespace Sofco.Service.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Response<User> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public IList<User> GetAllReadOnly()
        {
            throw new NotImplementedException();
        }

        public Response<User> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Response<User> Insert(User role)
        {
            throw new NotImplementedException();
        }
    }
}
