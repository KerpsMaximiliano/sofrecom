using Sofco.Core.Models.Admin;
using Sofco.Domain.AzureAd;
using Sofco.Domain.Users;
using Sofco.Domain.Utils;

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
