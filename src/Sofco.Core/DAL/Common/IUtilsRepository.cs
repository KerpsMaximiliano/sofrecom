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
    }
}
