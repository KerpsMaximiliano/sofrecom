using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sofco.Domain.Models.Providers
{
    public class ProvidersAreaProviders 
    {
      [Key]
      public int Id { get; set; }
      public int ProviderAreaId {get;set;}  
      public int ProviderId { get; set; }
      public Providers Provider { get; set; }

    }
}
