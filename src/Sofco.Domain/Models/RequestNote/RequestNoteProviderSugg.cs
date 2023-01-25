using Sofco.Domain.Models.Common;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteProviderSugg : BaseEntity
    {
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
        public int ProviderId { get; set; }
        public Providers.Providers Provider { get; set; }
        public int? FileId { get; set; }
        public File File { get; set; }
    }
}
