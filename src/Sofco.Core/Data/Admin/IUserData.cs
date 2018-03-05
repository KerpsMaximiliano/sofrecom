using Sofco.Model.Models.Admin;

namespace Sofco.Core.Data.Admin
{
    public interface IUserData
    {
        User GetByManagerId(string managerId);

        User GetById(int userId);

        User GetByUserName(string userName);
    }
}