using Sofco.Model.AzureAd;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Admin
{
    public interface IAzureService
    {
        Response<AzureAdUserListResponse> GetAllUsersActives();
    }
}
