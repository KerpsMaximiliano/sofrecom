using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Provider;
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
