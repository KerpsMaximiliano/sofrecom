using Sofco.Domain.Models.AllocationManagement;
using System;


namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteTrainingEmployee //: BaseEntity
    {
        public int RequestNoteTainingId { get; set; }
        public RequestNoteTraining RequestNoteTraining { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}

