using Sofco.Domain.Models.Common;

namespace Sofco.Domain.Models.Recruitment
{
    public class ApplicantFile
    {
        public int FileId { get; set; }
        public File File { get; set; }

        public int ApplicantId { get; set; }
        public Applicant Applicant { get; set; }
    }
}
