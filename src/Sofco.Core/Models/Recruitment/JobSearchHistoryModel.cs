using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Recruitment;

namespace Sofco.Core.Models.Recruitment
{
    public class JobSearchHistoryModel
    {
        public JobSearchHistoryModel(JobSearchHistory domain)
        {
            Comment = domain.Comment;
            CreatedDate = domain.CreatedDate;
            StatusFromId = domain.StatusFromId;
            StatusToId = domain.StatusToId;
            UserName = domain.UserName;
            ReasonCause = domain.ReasonCause?.Text;
        }

        public JobSearchStatus StatusFromId { get; }
        public JobSearchStatus StatusToId { get; }
        public string UserName { get; }
        public string ReasonCause { get; }
        public string Comment { get; }
        public DateTime CreatedDate { get; }
    }
}
