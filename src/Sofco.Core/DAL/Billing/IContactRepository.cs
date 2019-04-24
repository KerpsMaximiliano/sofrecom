using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface IContactRepository : IBaseRepository<Contact>
    {
        Contact GetByAccountId(string crmContactId);
        Contact GetByCrmId(string crmId);
    }
}
