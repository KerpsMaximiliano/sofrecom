using Sofco.Model.Utils;
using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Services.Admin
{
    public interface IUserService
    {
        IList<User> GetAllReadOnly(bool active);

        Response<User> GetById(int id);

        Response<User> Active(int id, bool active);

        Response<User> AddUserGroup(int userId, int userGroupId);

        Response<User> RemoveUserGroup(int userId, int userGroupId);

        Response<User> ChangeUserGroups(int userId, List<int> groupsToAdd, List<int> groupsToRemove);

        Response<UserModel> GetByMail();

        bool HasDirectorGroup();

        Response Add(User domain);

        Response CheckIfExist(string mail);

        bool HasDafGroup();

        bool HasCdgGroup();
        ICollection<User> GetManagers();

        bool HasRrhhGroup();

        bool HasManagerGroup();

        IList<User> GetAuthorizers();
    }
}
