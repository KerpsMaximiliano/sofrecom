using System;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantFileModel
    {
        public ApplicantFileModel(ApplicantFile applicantFile)
        {
            if (applicantFile.File != null)
            {
                Name = applicantFile.File.FileName;
                Date = applicantFile.File.CreationDate;
                UserName = applicantFile.File.CreatedUser;
                Id = applicantFile.FileId;
            }
        }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string UserName { get; set; }

        public int Id { get; set; }
    }
}
