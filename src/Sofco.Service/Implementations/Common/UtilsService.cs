using Sofco.Core.Services.Common;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Model.Enums;
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

        public IList<Sector> GetSectors()
        {
            return unitOfWork.UtilsRepository.GetSectors();
        }

        public IList<EmployeeEndReason> GetEmployeeTypeEndReasons()
        {
            return unitOfWork.UtilsRepository.GetEmployeeTypeEndReasons();
        }

        public IEnumerable<Option> GetMonths()
        {
            yield return new Option { Id = 1, Text = Resources.Months.January };
            yield return new Option { Id = 2, Text = Resources.Months.February };
            yield return new Option { Id = 3, Text = Resources.Months.March };
            yield return new Option { Id = 4, Text = Resources.Months.April };
            yield return new Option { Id = 5, Text = Resources.Months.May };
            yield return new Option { Id = 6, Text = Resources.Months.June };
            yield return new Option { Id = 7, Text = Resources.Months.July };
            yield return new Option { Id = 8, Text = Resources.Months.August };
            yield return new Option { Id = 9, Text = Resources.Months.September };
            yield return new Option { Id = 10, Text = Resources.Months.October };
            yield return new Option { Id = 11, Text = Resources.Months.November };
            yield return new Option { Id = 12, Text = Resources.Months.December };
        }

        public IEnumerable<Option> GetYears()
        {
            for (int i = 2018; i < 2099; i++)
            {
                yield return new Option { Id = i, Text = i.ToString() };
            }
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
