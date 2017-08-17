using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services
{
    public interface IUserService
    {
        IList<User> GetAllFullReadOnly();
        IList<User> GetAllReadOnly(bool active);
        Response<User> GetById(int id);
        Response<User> Active(int id, bool active);
        Response<User> AddUserGroup(int userId, int userGroupId);
        Response<User> RemoveUserGroup(int userId, int userGroupId);
        Response<User> ChangeUserGroups(int userId, List<int> groupsToAdd, List<int> groupsToRemove);
    }
}
