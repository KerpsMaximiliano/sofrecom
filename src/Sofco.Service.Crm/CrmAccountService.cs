using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Service.Crm
{
    public class CrmAccountService : ICrmAccountService
    {
        private readonly ICrmApiHttpClient httpClient;

        public CrmAccountService(ICrmApiHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public List<CrmAccount> GetAll()
        {
            var result = httpClient.GetMany<CrmAccount>("accounts");

            return result.Data;
        }
    }
}
