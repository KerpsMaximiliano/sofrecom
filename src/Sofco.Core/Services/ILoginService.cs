using Sofco.Common.Domains;
using Sofco.Core.Models.Admin;
using Sofco.Model.AzureAd;
using Sofco.Model.Users;
using Sofco.Model.Utils;

namespace Sofco.Core.Services
{
    public interface ILoginService
    {
        Response<UserTokenModel> Login(UserLogin userLogin);

        Response<UserTokenModel> Refresh(UserLoginRefresh userLoginRefresh);

        Response<AzureAdUserResponse> GetUserFromAzureAdByEmail(string email);

        Response<AzureAdUserListResponse> GetUsersFromAzureAdBySurname(string surname);
    }
}
