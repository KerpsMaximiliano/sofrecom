using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface ICertificateRepository : IBaseRepository<Certificate>
    {
        Certificate GetById(int certificateId);
        bool Exist(int domainId);
        ICollection<Certificate> Search(SearchCertificateParams parameters);
    }
}
