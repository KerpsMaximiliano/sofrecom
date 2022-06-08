using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.DTO.NotaPedido
{
    public class RequestNoteLoadNuevoDTO : RequestNoteLoadDTO
    {
        public string Description { get; set; }
        public int ProviderAreaId { get; set; }
        public bool RequiresEmployeeClient { get; set; }
        public bool ConsideredInBudget { get; set; }
        public int EvalpropNumber { get; set; }
        public string Comments { get; set; }
        // Completar resto de campos
    }
}
