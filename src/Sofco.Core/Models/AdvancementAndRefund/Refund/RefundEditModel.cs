using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundEditModel
    {
        public RefundEditModel(Domain.Models.AdvancementAndRefund.Refund refund)
        {
            Id = refund.Id;

            UserApplicantId = refund.UserApplicantId;
            UserApplicantDesc = refund.UserApplicant?.Name;

            CurrencyId = refund.CurrencyId;
            CurrencyDesc = refund.Currency?.Text;

            StatusId = refund.StatusId;
            StatusDesc = refund.Status?.Name;
            WorkflowStateType = refund.Status?.Type;

            AnalyticId = refund.AnalyticId;

            Details = new List<RefundEditDetailModel>(); 
            AdvancementIds = new List<int>();
            Advancements = new List<AdvancementUnrelatedItem>();
            Files = new List<Option>();

            foreach (var advancementRefund in refund.AdvancementRefunds)
            {
                AdvancementIds.Add(advancementRefund.AdvancementId);

                Advancements.Add(new AdvancementUnrelatedItem
                {
                    Id = advancementRefund.AdvancementId,
                    CurrencyId = refund.CurrencyId,
                    CurrencyText = refund.Currency?.Text,
                    Ammount = advancementRefund.Advancement.Ammount,
                    Text = $"{advancementRefund.Advancement.CreationDate:dd/MM/yyyy} - {advancementRefund.Advancement.Ammount} {refund.Currency?.Text}"
                });
            }

            foreach (var detail in refund.Details)
            {
                Details.Add(new RefundEditDetailModel(detail));
            }

            foreach (var refundAttachment in refund.Attachments)
            {
                Files.Add(new Option { Id = refundAttachment.FileId, Text = refundAttachment.File.FileName });
            }
        }

        public int Id { get; set; }

        public int UserApplicantId { get; set; }

        public string UserApplicantDesc { get; set; }

        public WorkflowStateType? WorkflowStateType { get; set; }

        public string StatusDesc { get; set; }

        public int StatusId { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyDesc { get; set; }

        public int AnalyticId { get; set; }

        public IList<int> AdvancementIds { get; set; }

        public IList<AdvancementUnrelatedItem> Advancements { get; set; }

        public IList<RefundEditDetailModel> Details { get; set; }

        public IList<Option> Files { get; set; }
    }
}
