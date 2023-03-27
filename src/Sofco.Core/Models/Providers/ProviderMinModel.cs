using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.Providers
{
    public class ProviderMinModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CUIT { get; set; }
       
        public ProviderMinModel(Sofco.Domain.Models.Providers.Providers providers)
        {
            Id = providers.Id;
            Name = providers.Name;
            CUIT = providers.CUIT;
        }

    }
}



