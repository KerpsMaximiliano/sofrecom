using System;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class LicenseType : BaseEntity, IEntityDate
    {
        public int LicenseTypeNumber { get; set; }

        public string Description { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
