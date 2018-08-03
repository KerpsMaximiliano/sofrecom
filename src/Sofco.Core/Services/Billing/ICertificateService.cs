using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ICertificateService
    {
        Response<Certificate> Add(Certificate domain);
        Task<Response<File>> AttachFile(int certificateId, Response<File> response, IFormFile file, string userName);
        Response<Certificate> GetById(int id);
        Response Update(Certificate domain);
        ICollection<Certificate> Search(SearchCertificateParams parameters);
        Response DeleteFile(int id);
        ICollection<Certificate> GetByClient(string client);
        ICollection<SolfacCertificate> GetBySolfac(int id);
    }
}
