using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;

namespace Sofco.Core.DAL.Billing
{
    public interface ICertificateRepository : IBaseRepository<Certificate>
    {
        Certificate GetById(int certificateId);
        bool Exist(int domainId);
        ICollection<Certificate> Search(SearchCertificateParams parameters);
        ICollection<Certificate> GetByClients(string client);
        void RelateToSolfac(SolfacCertificate solfacCertificate);
        ICollection<SolfacCertificate> GetBySolfacs(int id);
    }
}
