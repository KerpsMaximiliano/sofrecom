using Sofco.Core.Services.Common;
using System.Collections.Generic;
using Sofco.Model.Utils;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;

namespace Sofco.Service.Implementations.Common
{
    public class UtilsService : IUtilsService
    {
        private readonly IUtilsRepository _repository;

        public UtilsService(IUtilsRepository repo)
        {
            _repository = repo;
        }

        public IList<Currency> GetCurrencies()
        {
            return _repository.GetCurrencies();
        }

        public IList<PaymentTerm> GetPaymentTerms()
        {
            return _repository.GetPaymentTerms();
        }

        public IList<DocumentType> GetDocumentTypes()
        {
            return _repository.GetDocumentTypes();
        }

        public IList<ImputationNumber> GetImputationNumbers()
        {
            return _repository.GetImputationNumbers();
        }

        public IList<Province> GetProvinces()
        {
            return _repository.GetProvinces();
        }
    }
}
