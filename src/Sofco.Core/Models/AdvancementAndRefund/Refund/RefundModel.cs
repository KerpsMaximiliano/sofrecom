using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Core.Models.AdvancementAndRefund.Refund
{
    public class RefundModel
    {
        public int Id { get; set; }

        public int? UserApplicantId { get; set; }

        public int? CurrencyId { get; set; }

        public string Contract { get; set; }

        public IList<int> Advancements { get; set; }

        public IList<RefundDetailModel> Details { get; set; }

        public Domain.Models.AdvancementAndRefund.Refund CreateDomain()
        {
            var domain = new Domain.Models.AdvancementAndRefund.Refund();

            domain.Details = new List<RefundDetail>();

            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.CurrencyId = CurrencyId.GetValueOrDefault();
            domain.CreationDate = DateTime.UtcNow.Date;
            domain.Contract = Contract;
            domain.TotalAmmount = Details.Sum(x => x.Ammount);

            foreach (var detail in Details)
            {
                domain.Details.Add(detail.CreateDomain());
            }

            return domain;
        }
    }

    public class RefundDetailModel
    {
        public int Id { get; set; }

        public DateTime? CreationDate { get; set; }

        public string Description { get; set; }

        public decimal Ammount { get; set; }

        public RefundDetail CreateDomain()
        {
            var domain = new RefundDetail();

            domain.Description = Description;
            domain.Ammount = Ammount;
            domain.CreationDate = CreationDate.GetValueOrDefault().Date;

            return domain;
        }
    }
}
