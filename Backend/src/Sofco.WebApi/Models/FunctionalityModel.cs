using Sofco.Model;
using Sofco.Model.Interfaces;
using Sofco.Model.Models;
using System;
using System.Collections.Generic;

namespace Sofco.WebApi.Models
{
    public class FunctionalityModel : BaseEntity, IAuditDates
    {
        public FunctionalityModel(Functionality functionality)
        {
            Id = functionality.Id;
            Description = functionality.Description;
            Active = functionality.Active;
            StartDate = functionality.StartDate;
            EndDate = functionality.EndDate;

            Roles = new List<RoleModel>();
        }

        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<RoleModel> Roles { get; set; }
    }
}
