using System;
using Sofco.Model;

namespace Sofco.Core.Models.AllocationManagement
{
    public class UnemployeeListItemModel : BaseEntity
    {
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string EndReasonType { get; set; }

        public string EndReasonComments { get; set; }
    }
}
