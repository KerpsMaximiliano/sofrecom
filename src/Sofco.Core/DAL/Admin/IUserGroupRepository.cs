﻿using Sofco.Core.DAL.Common;
using Sofco.Domain.Relationships;

namespace Sofco.Core.DAL.Admin
{
    public interface IUserGroupRepository : IBaseRepository<UserGroup>
    {
        bool ExistById(int userId, int groupId);
        int[] GetGroupsId(string userId);
    }
}
