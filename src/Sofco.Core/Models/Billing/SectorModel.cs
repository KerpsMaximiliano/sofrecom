using System;
using Sofco.Core.Models.Admin;

namespace Sofco.Core.Models.Billing
{
    public class SectorModel
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public UserLiteModel ResponsableUser { get; set; }
    }
}
