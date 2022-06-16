using System;


namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteTraining : BaseEntity
    {
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
        public DateTime TrainingDate { get; set; }
        public  string Subject { get; set; }
        public string Topic { get; set; }
        public string Place { get; set; }
        public string Duration { get; set; }
        public decimal Ammount { get; set; }
    }

}

