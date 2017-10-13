using Sofco.Common.Domains;
using Sofco.Model.AzureAd;
using Sofco.Model.Users;
using Sofco.Model.Utils;

namespace Sofco.Core.Services
{
    public interface ILoginService
    {
        Result Login(UserLogin userLogin);

        Result Refresh(UserLoginRefresh userLoginRefresh);

        Response<AzureAdUserResponse> GetUserFromAzureADByEmail(string email);

        Response<AzureAdUserListResponse> GetUsersFromAzureADBySurname(string surname);
    }
}
