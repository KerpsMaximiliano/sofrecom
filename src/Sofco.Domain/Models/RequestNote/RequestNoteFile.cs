using Sofco.Domain.Models.Common;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteFile
    {
        public int FileId { get; set; }
        public File File { get; set; }
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
    }
}
