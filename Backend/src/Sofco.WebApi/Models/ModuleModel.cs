using Sofco.Model;
using Sofco.Model.Interfaces;
using Sofco.Model.Models;
using System;
using System.Collections.Generic;

namespace Sofco.WebApi.Models
{
    public class ModuleModel : BaseEntity, IAuditDates
    {
        public ModuleModel(Module module)
        {
            Id = module.Id;
            Description = module.Description;
            Active = module.Active;
            StartDate = module.StartDate;
            EndDate = module.EndDate;

            Functionalities = new List<FunctionalityModel>();
        }

        public string Description { get; set; }

        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<FunctionalityModel> Functionalities { get; set; }
    }
}
