﻿using Sofco.Core.Models.Admin;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.Data.Admin
{
    public interface IUserData
    {
        User GetByExternalManagerId(string externalManagerId);

        User GetById(int userId);

        User GetByUserName(string userName);

        UserLiteModel GetUserLiteById(int id);

        UserLiteModel GetUserLiteByUserName(string userName);

        UserLiteModel GetCurrentUser();
    }
}