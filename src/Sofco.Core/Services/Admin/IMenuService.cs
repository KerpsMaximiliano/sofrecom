using Sofco.Core.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface IMenuService
    {
        Response<MenuResponseModel> GetFunctionalitiesByUserName();

        string GetGroupMail(string emailConfigDafCode);
    }
}
