using Sofco.Model.Utils;
using System.Collections.Generic;
using Sofco.Core.Models.Billing;

namespace Sofco.Core.Services.Common
{
    public interface IUtilsService
    {
        IList<Province> GetProvinces();

        IList<ImputationNumber> GetImputationNumbers();

        IList<DocumentType> GetDocumentTypes();

        IList<Currency> GetCurrencies();

        IList<PaymentTerm> GetPaymentTerms();

        IList<SectorModel> GetSectors();

        Response<List<SectorModel>> GetSectorsByCurrentUser();

        IList<EmployeeEndReason> GetEmployeeTypeEndReasons();

        IEnumerable<Option> GetMonths();

        IEnumerable<Option> GetYears();

        IList<AreaModel> GetAreas();

        Response<List<AreaModel>> GetAreasByCurrentUser();

        List<object> GetUserDelegateType();
    }
}
