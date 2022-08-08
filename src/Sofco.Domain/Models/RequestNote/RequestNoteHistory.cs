using Sofco.Domain.Interfaces;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteHistory : WorkflowHistory
    {
        public int RequestNoteId { get; set; }

        public RequestNote RequestNote { get; set; }

        public override void SetEntityId(int entityId)
        {
            this.RequestNoteId = entityId;
        }
    }
}
