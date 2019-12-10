using System;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class ApplicantFileModel
    {
        public ApplicantFileModel(JobSearchApplicantFile jobSearchApplicantFile)
        {
            JobSearch = jobSearchApplicantFile.JobSearchId;

            if (jobSearchApplicantFile.File != null)
            {
                Name = jobSearchApplicantFile.File.FileName;
                Date = jobSearchApplicantFile.File.CreationDate;
                UserName = jobSearchApplicantFile.File.CreatedUser;
                Id = jobSearchApplicantFile.FileId;
            }
        }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string UserName { get; set; }

        public int Id { get; set; }

        public int JobSearch { get; set; }
    }
}
