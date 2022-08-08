using Sofco.Domain.Models.AllocationManagement;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteTravelEmployee //: BaseEntity
    {

        [Key, Column(Order = 0)]
        public int EmployeeId { get; set; }
        [Key, Column(Order = 1)]
        public int RequestNoteTravelId { get; set; }
        public RequestNoteTravel RequestNoteTravel { get; set; }
        public Employee Employee { get; set; }
    }
}

