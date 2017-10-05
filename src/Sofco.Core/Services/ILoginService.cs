using Sofco.Common.Domains;
using Sofco.Model.Users;

namespace Sofco.Core.Services
{
    public interface ILoginService
    {
        Result Login(UserLogin userLogin);

        Result Refresh(UserLoginRefresh userLoginRefresh);

        Result GetUserFromAzureAD(string email, string token);
    }
}
