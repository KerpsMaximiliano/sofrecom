using System;
using System.Collections.Generic;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteTravel : BaseEntity
    {
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public  string Destination { get; set; }
        public string Conveyance { get; set; }
        public string Accommodation { get; set; }
        public string ItineraryDetail { get; set; }


        public IList<RequestNoteTravelEmployee> Employees { get; set; }
    }
}

