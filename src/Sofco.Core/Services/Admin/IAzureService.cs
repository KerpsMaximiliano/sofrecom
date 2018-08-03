using Sofco.Domain.AzureAd;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface IAzureService
    {
        Response<AzureAdUserListResponse> GetAllUsersActives();
    }
}
