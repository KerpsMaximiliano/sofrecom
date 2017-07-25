using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services
{
    public interface IUserService
    {
        IList<User> GetAllReadOnly();
        Response<User> GetById(int id);
        Response<User> Insert(User role);
        Response<User> DeleteById(int id);
    }
}
