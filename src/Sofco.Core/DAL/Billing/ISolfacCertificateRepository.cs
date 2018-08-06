using Sofco.Core.DAL.Common;
using Sofco.Domain.Relationships;

namespace Sofco.Core.DAL.Billing
{
    public interface ISolfacCertificateRepository : IBaseRepository<SolfacCertificate>
    {
        bool HasCertificates(int solfacId);
        bool Exist(int id, int certificateId);
    }
}
