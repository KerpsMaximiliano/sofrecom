﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Admin;
using Sofco.Domain.Models.Admin;

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
        bool HasPmoGroup(string userMail);
        bool HasCdgGroup(string userMail);
        bool IsActive(string userMail);
        IList<User> GetDirectors();

        IList<User> GetManagers();

        IList<User> GetCommercialManagers();

        IList<User> GetSellers();
        bool HasComercialGroup(string comercialCode, string email);

        bool HasComercialGroup(string email);

        bool HasManagerGroup(string userName);

        bool HasRrhhGroup(string userMail);

        UserLiteModel GetUserLiteById(int userId);

        bool HasManagersGroup(string userMail);

        UserLiteModel GetUserLiteByUserName(string userName);

        User GetByEmail(string email);

        IList<User> GetAuthorizers();

        bool HasComplianceGroup(string userEmail);

        IList<User> GetByGroup(string groupCode);

        bool HasDafPurchaseOrderGroup(string userMail);

        IList<User> GetExternalsFree();

        List<UserLiteModel> GetUserLiteByIds(List<int> userIds);

        bool HasGafGroup(string email);

        List<UserLiteModel> GetUserLiteByEmails(List<string> emails);

        bool HasReadOnlyGroup(string currentUserEmail);

        IList<User> GetByIdsWithGroups(IEnumerable<int> ids);
        bool HasSensibleDataGroup(string currentUserEmail);
        bool HasManagementReportDelegateGroup(string currentUserEmail);
        bool IsRecruiter(string currentUserEmail, string recruitersCode);
        IList<User> GetByEmail(List<string> mails);

        bool HasAdminGroup(string userMail);

        bool HasPermission(int userId, string codigoFuncionalidad, string codigoModulo);

        List<string> GetPermissions(int userId, string codigoModulo);

        IList<User> GetActivesByGroup(string groupCode);


    }
}
