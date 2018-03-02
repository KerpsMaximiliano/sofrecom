using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Model.Models.Common;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseService
    {
        Response<string> Add(License createDomain);
        Task<Response<File>> AttachFile(int id, Response<File> response, IFormFile file);
    }
}
