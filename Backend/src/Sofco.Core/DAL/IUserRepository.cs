﻿using System;
using Sofco.Core.Interfaces.DAL;
using Sofco.Model.Models;
using System.Linq.Expressions;

namespace Sofco.Core.DAL
{
    public interface IUserRepository : IBaseRepository<User>
    {
        bool ExistById(int id);
        User GetSingleWithUserGroup(Expression<Func<User, bool>> predicate);
    }
}
