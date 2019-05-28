using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Core.FileManager
{
    public interface IPrepaidFileManager
    {
        Response<PrepaidDashboard> Process(int yearId, int monthId, IFormFile file, Prepaid prepaid);
    }
}
