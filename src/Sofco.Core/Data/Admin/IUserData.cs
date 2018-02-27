using Sofco.Model.Models.Admin;

namespace Sofco.Core.Data.Admin
{
    public interface IUserData
    {
        User GetById(int userId);
    }
}