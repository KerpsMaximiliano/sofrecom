using System.Linq;
using AutoMapper;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Domain.Models.AdvancementAndRefund;

namespace Sofco.Service.MapProfiles
{
    public class RefundMapProfile : Profile
    {
        public RefundMapProfile()
        {
            CreateMap<Refund, RefundListResultModel>()
                .ForMember(s => s.UserApplicantName, x => x.MapFrom(_ => _.UserApplicant.Name))
                .ForMember(s => s.CurrencyName, x => x.MapFrom(_ => _.Currency.Text))
                .ForMember(s => s.AdvancementSum, x => x.ResolveUsing(ResolveAdvancementSum))
                .ForMember(s => s.RefundItemTotal, x => x.ResolveUsing(ResolveRefundItemTotal))
                .ForMember(s => s.WorkflowStatusType, x => x.ResolveUsing(_ => _.Status?.Type))
                .ForMember(s => s.ManagerName, x => x.ResolveUsing(_ => _.Analytic?.Manager?.Name))
                .ForMember(s => s.StatusName, x => x.ResolveUsing(_ => _.Status?.Name));

            CreateMap<Refund, EmployeeRefundDetail>()
                .ForMember(s => s.CurrencyName, x => x.MapFrom(_ => _.Currency.Text))
                .ForMember(s => s.IsCreditCard, x => x.MapFrom(_ => _.CreditCardId.HasValue))
                .ForMember(s => s.IsCashReturn, x => x.MapFrom(_ => _.CashReturn))
                .ForMember(s => s.AdvancementSum, x => x.ResolveUsing(ResolveAdvancementSum))
                .ForMember(s => s.RefundItemTotal, x => x.ResolveUsing(ResolveRefundItemTotal))
                .ForMember(s => s.WorkflowStatusType, x => x.ResolveUsing(_ => _.Status?.Type))
                .ForMember(s => s.StatusName, x => x.ResolveUsing(_ => _.Status?.Name));

            CreateMap<Refund, RefundPaymentPendingModel>()
                .ForMember(s => s.CurrencyName, x => x.ResolveUsing(_ => _.Currency?.Text))
                .ForMember(s => s.CreationDate, x => x.MapFrom(_ => _.CreationDate))
                .ForMember(s => s.WorkflowId, x => x.MapFrom(_ => _.WorkflowId))
                .ForMember(s => s.UserApplicantId, x => x.MapFrom(_ => _.UserApplicantId))
                .ForMember(s => s.UserApplicantDesc, x => x.ResolveUsing(_ => _.UserApplicant?.Name))
                .ForMember(s => s.AdvancementSum, x => x.ResolveUsing(ResolveAdvancementSum))
                .ForMember(s => s.RefundItemTotal, x => x.ResolveUsing(ResolveRefundItemTotal))
                .ForMember(s => s.Id, x => x.ResolveUsing(_ => _.Id));
        }

        private decimal ResolveAdvancementSum(Refund refund)
        {
            return refund.AdvancementRefunds.Select(x => x.Advancement).Sum(x => x.Ammount);
        }

        private decimal ResolveRefundItemTotal(Refund refund)
        {
            return refund.Details.Sum(x => x.Ammount);
        }
    }
}
