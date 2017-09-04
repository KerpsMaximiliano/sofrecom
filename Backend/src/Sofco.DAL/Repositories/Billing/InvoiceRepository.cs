using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Billing;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.Billing;

namespace Sofco.DAL.Repositories.Billing
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(SofcoContext context) : base(context)
        {
        }

        public IList<Invoice> GetByProject(string projectId)
        {
            return _context.Invoices.Where(x => x.ProjectId == projectId).ToList();
        }

        public Invoice GetById(int id)
        {
            return _context.Invoices.Include(x => x.Details).SingleOrDefault(x => x.Id == id);
        }
    }
}
