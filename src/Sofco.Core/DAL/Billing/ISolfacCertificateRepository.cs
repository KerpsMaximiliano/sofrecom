using Sofco.Core.DAL.Common;
using Sofco.Model.Relationships;

namespace Sofco.Core.DAL.Billing
{
    public interface ISolfacCertificateRepository : IBaseRepository<SolfacCertificate>
    {
        bool HasCertificates(int solfacId);
    }
}
