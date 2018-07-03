using System;
using System.Collections.Generic;
using AutoMapper;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
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
        private readonly ISessionManager sessionManager;
        private readonly IMapper mapper;
        private List<UserDelegateType> types;

        public PurchaseOrderDelegateService(IUnitOfWork unitOfWork, IUserData userData, ISessionManager sessionManager, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.sessionManager = sessionManager;
            this.mapper = mapper;
            SetTypes();
        }

        public Response<List<PurchaseOrderDelegateModel>> GetAll()
        {
            var data = unitOfWork.UserDelegateRepository.GetByTypes(types);

            var result = Translate(data);

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

        private List<PurchaseOrderDelegateModel> Translate(List<UserDelegate> userDelegates)
        {
            return mapper.Map<List<UserDelegate>, List<PurchaseOrderDelegateModel>>(userDelegates);
        }
    }
}
