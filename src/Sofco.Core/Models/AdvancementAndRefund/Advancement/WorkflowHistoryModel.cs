using System;
using Sofco.Domain.Enums;
using Sofco.Domain.Interfaces;

namespace Sofco.Core.Models.AdvancementAndRefund.Advancement
{
    public class WorkflowHistoryModel
    {
        public WorkflowHistoryModel(WorkflowHistory history)
        {
            CreatedDate = history.CreatedDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
            UserName = history.UserName;

            StatusFrom = history.StatusFrom?.Name;
            StatusTo = history.StatusTo?.Name;

            StatusFromType = history.StatusFrom?.Type;
            StatusToType = history.StatusTo?.Type;

            Comment = history.Comment;
        }

        public string CreatedDate { get; set; }

        public string UserName { get; set; }

        public string StatusFrom { get; set; }

        public string StatusTo { get; set; }

        public string Comment { get; set; }

        public WorkflowStateType? StatusToType { get; set; }

        public WorkflowStateType? StatusFromType { get; set; }
    }
}
