using System.Collections.Generic;

namespace Sofco.Core.Data.Billing
{
    public interface IInvoiceData
    {
        void AddInvoice(string username, int id);
        IList<int> GetAll(string username);
        void ClearKeys();
    }
}
