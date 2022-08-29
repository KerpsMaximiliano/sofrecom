using Sofco.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.Providers
{
    public class ProvidersArea : BaseEntity
    {
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool Critical { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<RequestNote.RequestNote> RequestNotes { get; set; }

    }
}
