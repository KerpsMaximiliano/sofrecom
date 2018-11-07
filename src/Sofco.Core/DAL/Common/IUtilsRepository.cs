using System.Collections.Generic;
using Sofco.Domain.Utils;

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
        IList<PurchaseOrderOptions> GetPurchaseOrderOptions();
        IList<SoftwareLaw> GetSoftwareLaws();
        IList<ServiceType> GetServiceTypes();
        IList<Sector> GetSectors();
        IList<EmployeeEndReason> GetEmployeeTypeEndReasons();
        IList<Area> GetAreas();
        IList<AdvancementReturnForm> GetAdvancementReturnForms();
        bool ExistAdvancementReturnForm(int id);
        bool ExistCurrency(int currencyId);
    }
}
