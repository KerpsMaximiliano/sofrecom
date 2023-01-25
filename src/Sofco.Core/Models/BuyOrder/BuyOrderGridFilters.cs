using Newtonsoft.Json;
using Sofco.Domain.RequestNoteStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Core.Models.BuyOrder
{
    
    public class BuyOrderGridFilters
    {
        public BuyOrderGridFilters() { }
        
        public int? Id { get; set; }

        public int? RequestNoteId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? ProviderId { get; set; }
        
        public int? StatusId { get; set; }

        public string Number { get; set; }
    }
   
}
