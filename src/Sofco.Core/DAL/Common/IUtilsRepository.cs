using System.Collections.Generic;
using Sofco.Model.Utils;

namespace Sofco.Core.DAL.Common
{
    public interface IUtilsRepository
    {
        IList<Province> GetProvinces();
        IList<ImputationNumber> GetImputationNumbers();
        IList<DocumentType> GetDocumentTypes();
        IList<Currency> GetCurrencies();
        IList<PaymentTerm> GetPaymentTerms();
        IList<Solution> GetSolutions();
        IList<Technology> GetTechnologies();
        IList<Product> GetProducts();
        IList<ClientGroup> GetClientGroups();
        IList<PurchaseOrder> GetPurchaseOrders();
        IList<SoftwareLaw> GetSoftwareLaws();
        IList<ServiceType> GetServiceTypes();
        IList<Sector> GetSectors();
    }
}
