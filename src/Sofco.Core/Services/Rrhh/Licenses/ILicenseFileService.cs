using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Rrhh.Licenses
{
    public interface ILicenseFileService
    {
        Task<Response<File>> AttachFile(int id, Response<File> response, IFormFile file);

        Response DeleteFile(int id);

        Response<byte[]> GetLicenseReport(ReportParams parameters);

        Response FileDelivered(int id);
    }
}
