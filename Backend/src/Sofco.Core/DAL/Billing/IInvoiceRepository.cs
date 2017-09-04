using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface IInvoiceRepository : IBaseRepository<Invoice>
    {
        IList<Invoice> GetByProject(string projectId);
        Invoice GetById(int id);
    }
}
