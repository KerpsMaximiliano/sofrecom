using System;
using System.Collections.Generic;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteComment : BaseEntity
    {
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
        public DateTime Date { get; set; }
        public  string UserName { get; set; }
        public string Comment { get; set; }
       
    }
}

