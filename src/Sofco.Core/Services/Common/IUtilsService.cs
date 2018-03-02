using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services.Common
{
    public interface IUtilsService
    {
        IList<Province> GetProvinces();
        IList<ImputationNumber> GetImputationNumbers();
        IList<DocumentType> GetDocumentTypes();
        IList<Currency> GetCurrencies();
        IList<PaymentTerm> GetPaymentTerms();
        IList<Sector> GetSectors();
    }
}
