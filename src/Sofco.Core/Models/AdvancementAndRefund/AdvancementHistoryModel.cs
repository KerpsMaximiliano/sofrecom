using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.Models.AdvancementAndRefund
{
    public class AdvancementHistoryModel
    {
        public AdvancementHistoryModel(AdvancementHistory history)
        {
            CreatedDate = history.CreatedDate.ToString("dd/MM/yyyy");
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
