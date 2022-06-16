using Sofco.Core.Models.Providers;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.DAL.Provider
{
    public interface IProvidersRepository
    {

        IList<Domain.Models.Providers.Providers> GetAll();

        Domain.Models.Providers.Providers GetById(int providersId);

        void Update(Domain.Models.Providers.Providers providers);

        void Add(Domain.Models.Providers.Providers providers);


    }
}
