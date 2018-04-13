using Sofco.Core.Models.Admin;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Data.Admin
{
    public interface IUserData
    {
        User GetByExternalManagerId(string externalManagerId);

        User GetById(int userId);

        User GetByUserName(string userName);

        UserLiteModel GetUserLiteById(int id);
    }
}