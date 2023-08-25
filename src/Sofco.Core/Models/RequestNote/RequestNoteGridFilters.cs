using Newtonsoft.Json;
using Sofco.Domain.RequestNoteStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Core.Models.RequestNote
{
    
    public class RequestNoteGridFilters
    {
        public RequestNoteGridFilters() { }
        
        public int? Id { get; set; }

        public int? CreationUserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ProviderId { get; set; }
        public int? AnalyticID { get; set; }
        public int? StatusId { get; set; }
    }
   
}
