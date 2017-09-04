using System.Collections.Generic;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IInvoiceService
    {
        IList<Invoice> GetByProject(string projectId);
        Response<Invoice> GetById(int id);
        Response<Invoice> Add(Invoice invoice);
    }
}
