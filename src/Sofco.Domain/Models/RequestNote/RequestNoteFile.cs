using Sofco.Domain.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace Sofco.Domain.Models.RequestNote
{
    public class RequestNoteFile
    {
        [Key]
        public int FileId { get; set; }
        public File File { get; set; }
        [Key]
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
    }
}
