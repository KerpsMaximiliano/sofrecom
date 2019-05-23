using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface IPrepaidService
    {
        Response Import(int prepaidId, int yearId, int monthId, IFormFile file);
        Response<IList<PrepaidDashboard>> GetDashboard(int yearId, int monthId);
    }
}
