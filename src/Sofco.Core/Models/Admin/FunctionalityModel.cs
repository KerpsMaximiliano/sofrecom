﻿using Sofco.Domain;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.Models.Admin
{
    public class FunctionalityModel : BaseEntity
    {
        public FunctionalityModel(Functionality functionality)
        {
            Id = functionality.Id;
            Description = functionality.Description;
            Active = functionality.Active;
        }

        public string Description { get; set; }

        public bool Active { get; set; }
    }
}