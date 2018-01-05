using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.DAL.Admin
{
    public interface IUserRepository : IBaseRepository<User>
    {
        bool ExistById(int id);
        User GetSingleWithUserGroup(Expression<Func<User, bool>> predicate);
        IList<User> GetAllFullReadOnly();
        IList<User> GetAllActivesReadOnly();
        bool HasDirectorGroup(string userMail);
        Group GetGroup(int solfacUserApplicantId);
        bool ExistByMail(string mail);
        bool HasDafGroup(string userMail, string dafCode);
        bool HasCdgGroup(string userMail, string cdgCode);
        bool IsActive(string userMail);
        IList<User> GetDirectors();
        IList<User> GetManagers();
        IList<User> GetSellers(string sellerCode);
    }
}
