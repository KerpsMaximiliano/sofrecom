using System;
using System.Collections.Generic;
using Sofco.Core.Cache;
using Sofco.Core.Data.Billing;

namespace Sofco.Data.Billing
{
    public class InvoiceData : IInvoiceData
    {
        private const string CacheKey = "urn:invoices:{0}:all";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(60);

        private readonly ICacheManager cacheManager;

        public InvoiceData(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public void AddInvoice(string username, int id)
        {
            var invoices = GetAll(username);

            invoices.Add(id);

            cacheManager.Set(string.Format(CacheKey, username), invoices, cacheExpire);
        }

        public IList<int> GetAll(string username)
        {
            return cacheManager.Get(string.Format(CacheKey, username),
                () => new List<int>(), 
                cacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(CacheKey, '*'));
        }
    }
}
