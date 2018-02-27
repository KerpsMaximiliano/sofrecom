using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Models.Common;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;

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
