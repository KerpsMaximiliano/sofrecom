using System;
using Sofco.Domain.Models.Common;

namespace Sofco.Domain.Models.Recruitment
{
    public class JobSearchApplicantFile
    {
        public int FileId { get; set; }
        public File File { get; set; }

        public int JobSearchId { get; set; }
        public int ApplicantId { get; set; }
        public DateTime Date { get; set; }
        public JobSearchApplicant JobSearchApplicant { get; set; }
    }
}
