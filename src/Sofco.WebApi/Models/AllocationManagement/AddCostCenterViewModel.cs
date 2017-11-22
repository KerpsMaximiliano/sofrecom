﻿using Sofco.Model.Models.AllocationManagement;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class AddCostCenterViewModel
    {
        public int Code { get; set; }
        public string Letter { get; set; }
        public string Description { get; set; }

        public CostCenter CreateDomain()
        {
            var domain = new CostCenter();

            domain.Code = Code;
            domain.Letter = Letter;
            domain.Description = Description;
            domain.Active = true;

            return domain;
        }
    }
}
