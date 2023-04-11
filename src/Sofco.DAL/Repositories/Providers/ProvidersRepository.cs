using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Provider;
using Sofco.Core.Models.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.DAL.Repositories.Providers
{
    public class ProvidersRepository : IProvidersRepository
    {
        protected readonly SofcoContext context;

        public ProvidersRepository(SofcoContext context)
        {
            this.context = context;
        }

        public void Add(Domain.Models.Providers.Providers providers)
        {
            context.Providers.Add(providers);

        }

        public IList<Domain.Models.Providers.Providers> GetAll()
        {
            return this.context.Providers.Include(x =>x.ProvidersAreaProviders).ToList();
        }

        public IList<Domain.Models.Providers.Providers> GetByParams(ProvidersGetByParamsModel parameters)
        {
            return this.context.Providers.Include(x => x.ProvidersAreaProviders).Where(x => (!parameters.statusId.HasValue || Convert.ToInt16(x.Active) == parameters.statusId) &&
                                                    (String.IsNullOrEmpty(parameters.businessName) || EF.Functions.Like(x.Name, "%" + parameters.businessName + "%"))).ToList();

            //return this.context.Providers.Include(x => x.ProvidersAreaProviders).Where(x => (!parameters.statusId.HasValue || Convert.ToInt16(x.Active) == parameters.statusId) &&
            //                                         (String.IsNullOrEmpty(parameters.businessName) || EF.Functions.Like(x.Name, "%" + parameters.businessName + "%")) &&
            //                                         (parameters.providersArea == null || parameters.providersArea.Count ==0 || parameters.providersArea.Any(e => (x.ProvidersAreaProviders.Select(y => y.ProviderAreaId)).Contains(e)))).ToList();
        }

        public Domain.Models.Providers.Providers GetById(int providersId)
        {
            return context.Providers.Include(x => x.ProvidersAreaProviders)
                 .Where(x => x.Id == providersId).FirstOrDefault();
        }

        public void Update(Domain.Models.Providers.Providers providers)
        {
            context.Providers.Update(providers);
        }
    }
}
