using Sofco.Domain.Models.AllocationManagement;
using System;


namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteTravelEmployee : BaseEntity
    {
        public int RequestNoteTravelId { get; set; }
        public RequestNoteTravel RequestNoteTravel { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}

