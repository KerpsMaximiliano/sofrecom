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
                .ForMember(s => s.RefundSum, x => x.ResolveUsing(ResolveRefundSum))
                .ForMember(s => s.DifferenceSum, x => x.ResolveUsing(ResolveDifferenceSum));
        }

        private decimal ResolveAdvancementSum(Refund refund)
        {
            return refund.AdvancementRefunds.Sum(x => x.Advancement.Ammount);
        }

        private decimal ResolveRefundSum(Refund refund)
        {
            return refund.Details.Sum(x => x.Ammount);
        }

        private decimal ResolveDifferenceSum(Refund refund)
        {
            return refund.AdvancementRefunds.Sum(x => x.Advancement.Ammount) - refund.Details.Sum(x => x.Ammount);
        }
    }
}
