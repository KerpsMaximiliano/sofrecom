using Sofco.Model.Models;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Interfaces.Services
{
    public interface IGroupService
    {
        IList<Group> GetAllFullReadOnly();
        IList<Group> GetAllReadOnly();
        Response<Group> GetById(int id);
        Response<Group> Insert(Group role);
        Response<Group> DeleteById(int id);
        Response<Group> Update(Group role);
        Response<Group> AddRole(int roleId, int groupId);
        Response<Group> RemoveRole(int roleId, int groupId);
    }
}
