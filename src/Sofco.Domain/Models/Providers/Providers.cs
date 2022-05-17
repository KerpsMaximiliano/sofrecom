using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.Providers
{
    public class Providers : BaseEntity
    {
      public string  Name {get;set;}
      public int ProviderAreaId {get;set;}
      public int UserApplicantId { get; set; }
      public int UserEvaluatorId { get; set; }
      public int Score { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public bool Active { get; set; }
      public string CUIT { get; set; }
      public int IngresosBrutos { get; set; }
      public int CondicionIVA { get; set; }
      public string Address { get; set; }
      public string City { get; set; }
      public string ZIPCode { get; set; }
      public string Province { get; set; }
      public string ContactName { get; set; }
      public string Phone { get; set; }
      public string Email { get; set; }
      public string WebSite { get; set; }
      public string Comments { get; set; }

    }
}
