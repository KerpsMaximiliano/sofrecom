using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.DAL.Provider
{
    public interface IProviderAreaRepository
    {
        IList<Domain.Models.Providers.ProvidersArea> GetAll();

        Domain.Models.Providers.ProvidersArea GetById(int providersAreaId);

        void Update(Domain.Models.Providers.ProvidersArea providersArea);

        void Add(Domain.Models.Providers.ProvidersArea providersArea);
    }
}
