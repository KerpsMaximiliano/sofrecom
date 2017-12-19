using Sofco.Core.Services.Common;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Common
{
    public class UtilsService : IUtilsService
    {
        private readonly IUnitOfWork unitOfWork;

        public UtilsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IList<Currency> GetCurrencies()
        {
            return unitOfWork.UtilsRepository.GetCurrencies();
        }

        public IList<PaymentTerm> GetPaymentTerms()
        {
            return unitOfWork.UtilsRepository.GetPaymentTerms();
        }

        public IList<DocumentType> GetDocumentTypes()
        {
            return unitOfWork.UtilsRepository.GetDocumentTypes();
        }

        public IList<ImputationNumber> GetImputationNumbers()
        {
            return unitOfWork.UtilsRepository.GetImputationNumbers();
        }

        public IList<Province> GetProvinces()
        {
            return unitOfWork.UtilsRepository.GetProvinces();
        }
    }
}
