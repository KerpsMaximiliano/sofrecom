﻿using System.Collections.Generic;
using Sofco.Domain.Models;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface IGroupService
    {
        IList<Group> GetAllReadOnly(bool active);
        Response<Group> GetById(int id);
        Response<Group> Insert(Group role);
        Response<Group> DeleteById(int id);
        Response<Group> Update(Group group, int roleId);
        Response<Group> AddRole(int roleId, int groupId);
        Response<Group> RemoveRole(int roleId, int groupId);
        Response<Group> Active(int id, bool active);
    }
}
