using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Common;
using Sofco.Model.Enums.TimeManagement;
using Sofco.Model.Utils;

namespace Sofco.DAL.Repositories.Common
{
    public class UtilsRepository : IUtilsRepository
    {
        protected readonly SofcoContext _context;

        public UtilsRepository(SofcoContext context)
        {
            _context = context;
        }

        public IList<Currency> GetCurrencies()
        {
            return _context.Currencies.ToList().AsReadOnly();
        }

        public IList<PaymentTerm> GetPaymentTerms()
        {
            return _context.PaymentTerms.ToList().AsReadOnly();
        }

        public IList<Solution> GetSolutions()
        {
            return _context.Solutions.ToList().AsReadOnly();
        }

        public IList<Technology> GetTechnologies()
        {
            return _context.Technologies.ToList().AsReadOnly();
        }

        public IList<Product> GetProducts()
        {
            return _context.Products.ToList().AsReadOnly();
        }

        public IList<ClientGroup> GetClientGroups()
        {
            return _context.ClientGroups.ToList().AsReadOnly();
        }

        public IList<PurchaseOrderOptions> GetPurchaseOrderOptions()
        {
            return _context.PurchaseOrderOptions.ToList().AsReadOnly();
        }

        public IList<ServiceType> GetServiceTypes()
        {
            return _context.ServiceTypes.ToList().AsReadOnly();
        }

        public IList<Sector> GetSectors()
        {
            return _context.Sectors.ToList().AsReadOnly();
        }

        public IList<EmployeeEndReason> GetEmployeeTypeEndReasons()
        {
            return _context.EmployeeEndReason.ToList().AsReadOnly();
        }

        public IList<SoftwareLaw> GetSoftwareLaws()
        {
            return _context.SoftwareLaws.ToList().AsReadOnly();
        }

        public IList<DocumentType> GetDocumentTypes()
        {
            return _context.DocumentTypes.ToList().AsReadOnly();
        }

        public IList<ImputationNumber> GetImputationNumbers()
        {
            return _context.ImputationNumbers.ToList().AsReadOnly();
        }

        public IList<Province> GetProvinces()
        {
            return _context.Provinces.ToList().AsReadOnly();
        }
    }
}
