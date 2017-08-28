using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Common;
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
