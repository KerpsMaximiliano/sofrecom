using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Admin;
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
        bool HasDafGroup(string userMail);
        bool HasCdgGroup(string userMail);
        bool IsActive(string userMail);
        IList<User> GetDirectors();

        IList<User> GetManagers();

        IList<User> GetSellers();
        bool HasComercialGroup(string comercialCode, string email);
        bool HasComercialGroup(string email);
        bool HasManagerGroup(string userName);

        bool HasRrhhGroup(string userMail);

        UserLiteModel GetUserLiteById(int userId);

        bool HasManagersGroup(string userMail);

        UserLiteModel GetUserLiteByUserName(string userName);
    }
}
