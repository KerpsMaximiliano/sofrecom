using AutoMapper;
using Sofco.Core.Models.Billing;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Models.Common;
using Sofco.Model.Models.Common;

namespace Sofco.Service.MapProfiles
{
    public class UserDelegateMapProfile : Profile
    {
        public UserDelegateMapProfile()
        {
            CreateMap<UserDelegate, SolfacDelegateModel>();

            CreateMap<UserDelegate, UserDelegateModel>();

            CreateMap<UserDelegate, PurchaseOrderApprovalDelegateModel>();

            CreateMap<PurchaseOrderApprovalDelegateModel, UserDelegate>();
        }
    }
}
