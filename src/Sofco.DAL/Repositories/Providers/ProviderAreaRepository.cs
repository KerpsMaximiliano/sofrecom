using Sofco.Core.DAL.Provider;
using Sofco.Domain.Models.Providers;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Sofco.DAL.Repositories.Providers
{
    public class ProviderAreaRepository : IProviderAreaRepository
    {
        protected readonly SofcoContext context;

        public ProviderAreaRepository(SofcoContext context)
        {
            this.context = context;
        }

        public void Add(ProvidersArea providersArea)
        {
            context.ProvidersArea.Add(providersArea);
        }

        public IList<Domain.Models.Providers.ProvidersArea> GetAll()
        {
            return this.context.ProvidersArea.ToList();
        }

        public ProvidersArea GetById(int providersAreaId)
        {
            return context.ProvidersArea
                .Where(x => x.Id == providersAreaId).FirstOrDefault();

        }

        public void Update(ProvidersArea providersArea)
        {
                context.ProvidersArea.Update(providersArea);
        }
    }
}
