using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class ContactRepository : BaseRepository<Contact>, IContactRepository
    {
        public ContactRepository(SofcoContext context) : base(context)
        {
        }

        public Contact GetByCrmId(string crmId)
        {
            return context.Contacts.SingleOrDefault(x => x.CrmId.Equals(crmId));
        }

        public Contact GetByAccountId(string crmContactId)
        {
            return context.Contacts.FirstOrDefault(x => x.AccountId.Equals(crmContactId));
        }
    }
}
