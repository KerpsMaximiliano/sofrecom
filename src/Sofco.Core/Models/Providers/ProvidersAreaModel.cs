using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sofco.Domain;

namespace Sofco.Core.Models.Admin
{
    public class ProvidersAreaModel    
    {
        public ProvidersAreaModel(Sofco.Domain.Models.Providers.ProvidersArea providersArea) 
        {
            Id = providersArea.Id;
            Description = providersArea.Description;
            Critical = providersArea.Critical;
            Active = providersArea.Active;
            StartDate = providersArea.StartDate;
            EndDate = providersArea.EndDate;
            
        }
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Critical { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
