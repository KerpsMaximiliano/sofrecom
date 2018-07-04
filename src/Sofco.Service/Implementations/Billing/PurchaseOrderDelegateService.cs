using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class PurchaseOrderDelegateService : IPurchaseOrderDelegateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IAreaData areaData;
        private readonly ISectorData sectorData;
        private readonly IMapper mapper;
        private List<UserDelegateType> types;
        private Dictionary<UserDelegateType, Action<PurchaseOrderDelegateModel>> resolverSourceDicts;

        public PurchaseOrderDelegateService(IUnitOfWork unitOfWork, IUserData userData, IAreaData areaData, IMapper mapper, ISectorData sectorData)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userData = userData;
            this.areaData = areaData;
            this.sectorData = sectorData;
            SetTypes();
            SetResolverSourceDicts();
        }

        public Response<List<PurchaseOrderDelegateModel>> GetAll()
        {
            var data = unitOfWork.UserDelegateRepository.GetByTypes(types);

            var result = ResolveData(Translate(data));

            return new Response<List<PurchaseOrderDelegateModel>>{Data = result};
        }

        public Response<UserDelegate> Save(UserDelegate userDelegate)
        {
            throw new NotImplementedException();
        }

        public Response Delete(int userDeletegateId)
        {
            throw new NotImplementedException();
        }

        private void SetTypes()
        {
            types = new List<UserDelegateType>
            {
                UserDelegateType.PurchaseOrderCommercial,
                UserDelegateType.PurchaseOrderCompliance,
                UserDelegateType.PurchaseOrderDaf,
                UserDelegateType.PurchaseOrderOperation
            };
        }

        private void SetResolverSourceDicts()
        {
            resolverSourceDicts = new Dictionary<UserDelegateType, Action<PurchaseOrderDelegateModel>>
            {
                {UserDelegateType.PurchaseOrderCommercial, ResolveSourceCommercial},
                {UserDelegateType.PurchaseOrderOperation, ResolveSourceOperation}
            };
        }

        private List<PurchaseOrderDelegateModel> Translate(List<UserDelegate> userDelegates)
        {
            return mapper.Map<List<UserDelegate>, List<PurchaseOrderDelegateModel>>(userDelegates);
        }

        private List<PurchaseOrderDelegateModel> ResolveData(List<PurchaseOrderDelegateModel> userDelegates)
        {
            ResolveSource(userDelegates);

            ResolveUsers(userDelegates);

            return userDelegates;
        }

        private void ResolveSource(List<PurchaseOrderDelegateModel> userDelegates)
        {
            foreach (var userDelegate in userDelegates)
            {
                if (userDelegate.SourceId == 0) continue;

                if(!resolverSourceDicts.ContainsKey(userDelegate.Type)) continue;

                resolverSourceDicts[userDelegate.Type](userDelegate);
            }
        }

        private void ResolveSourceCommercial(PurchaseOrderDelegateModel purchaseOrderDelegate)
        {
            var area = areaData.GetAll().FirstOrDefault(s => s.Id == purchaseOrderDelegate.SourceId);

            if(area == null) return;

            purchaseOrderDelegate.SourceName = area.Text;

            purchaseOrderDelegate.ResponsableId = area.ResponsableUserId;
        }

        private void ResolveSourceOperation(PurchaseOrderDelegateModel purchaseOrderDelegate)
        {
            var area = sectorData.GetAll().FirstOrDefault(s => s.Id == purchaseOrderDelegate.SourceId);

            if(area == null) return;

            purchaseOrderDelegate.SourceName = area.Text;

            purchaseOrderDelegate.ResponsableId = area.ResponsableUserId;
        }

        private void ResolveUsers(List<PurchaseOrderDelegateModel> userDelegates)
        {
            foreach (var userDelegate in userDelegates)
            {
                var user = userData.GetUserLiteById(userDelegate.UserId);

                userDelegate.UserName = user?.Name;

                if(userDelegate.ResponsableId == 0) continue;

                var responsable = userData.GetUserLiteById(userDelegate.ResponsableId);

                userDelegate.ResponsableName = responsable?.Name;
            }
        }
    }
}
