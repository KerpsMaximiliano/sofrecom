using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing.PurchaseOrder;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.Billing;
using PurchaseOrderDomain = Sofco.Domain.Models.Billing.PurchaseOrder;

namespace Sofco.Service.Implementations.Billing.PurchaseOrder
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<PurchaseOrderService> logger;
        private readonly IUserData userData;
        private readonly IMapper mapper;

        public PurchaseOrderService(IUnitOfWork unitOfWork, 
            ILogMailer<PurchaseOrderService> logger, 
            IUserData userData, 
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            this.mapper = mapper;
        }

        public Response<PurchaseOrderDomain> Add(PurchaseOrderModel model)
        {
            var response = new Response<PurchaseOrderDomain>();

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain(userData.GetCurrentUser().UserName);

                var history = GetHistory(domain, new PurchaseOrderStatusParams());
                history.To = PurchaseOrderStatus.Draft;

                domain.Histories.Add(history);

                unitOfWork.PurchaseOrderRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.SaveSuccess); 
                
                response.Data = domain;
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
                return response;
            }

            try
            {
                foreach (var analyticId in model.AnalyticIds)
                {
                    var purchaseOrderAnalytic = new PurchaseOrderAnalytic { AnalyticId = analyticId, PurchaseOrderId = response.Data.Id };
                    unitOfWork.PurchaseOrderRepository.AddPurchaseOrderAnalytic(purchaseOrderAnalytic);
                }

                unitOfWork.Save();
            }
            catch (Exception e)
            {
                response.AddWarning(Resources.Billing.PurchaseOrder.ErrorToSaveAnalytics);
                logger.LogError(e);
            }

            return response;
        }

        public Response Update(PurchaseOrderModel model)
        {
            var response = new Response<PurchaseOrderDomain>();

            PurchaseOrderValidationHelper.Exist(response, model, unitOfWork);

            if (response.HasErrors()) return response;

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = PurchaseOrderValidationHelper.FindWithAnalytic(model.Id, response, unitOfWork);

                model.UpdateDomain(domain, userData.GetCurrentUser().UserName);

                var history = GetHistory(domain, new PurchaseOrderStatusParams());
                history.To = PurchaseOrderStatus.Draft;

                domain.Histories.Add(history);

                if (domain.Status == PurchaseOrderStatus.Reject)
                {
                    domain.Status = PurchaseOrderStatus.Draft;
                }
                
                var aux = domain.PurchaseOrderAnalytics.ToList();

                foreach (var orderAnalytic in aux)
                {
                    if (model.AnalyticIds.All(x => x != orderAnalytic.AnalyticId))
                    {
                        domain.PurchaseOrderAnalytics.Remove(orderAnalytic);
                    }
                }

                foreach (var analyticId in model.AnalyticIds)
                {
                    if (domain.PurchaseOrderAnalytics.All(x => x.AnalyticId != analyticId))
                    {
                        domain.PurchaseOrderAnalytics.Add(new PurchaseOrderAnalytic { AnalyticId = analyticId, PurchaseOrderId = domain.Id });
                    }
                }

                unitOfWork.PurchaseOrderRepository.Update(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.UpdateSuccess);

                response.Data = domain;
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }
                
        public ICollection<PurchaseOrderHistory> GetHistories(int id)
        {
            return unitOfWork.PurchaseOrderRepository.GetHistories(id);
        }

        public Response<IList<PurchaseOrderPendingModel>> GetPendings()
        {
            var currentUser = userData.GetCurrentUser();

            var purchaseOrders = unitOfWork.PurchaseOrderRepository.GetPendings();

            var isCdg = unitOfWork.UserRepository.HasCdgGroup(currentUser.Email);
            if (isCdg)
            {
                return new Response<IList<PurchaseOrderPendingModel>>
                {
                    Data = Translate(purchaseOrders.ToList())
                };
            }

            var isDaf = IsDaf(currentUser);
            var isCompliance = IsCompliance(currentUser);
            var commUserIds = GetCommercialUsers(currentUser);
            var operUserIds = GetOperationUsers(currentUser);

            var result = new List<PurchaseOrderDomain>();

            foreach (var purchaseOrder in purchaseOrders)
            {
                if (purchaseOrder.Status == PurchaseOrderStatus.CompliancePending 
                    && isCompliance)
                    result.Add(purchaseOrder);

                if (purchaseOrder.Status == PurchaseOrderStatus.ComercialPending
                    && purchaseOrder.Area != null
                    && commUserIds.Contains(purchaseOrder.Area.ResponsableUserId))
                    result.Add(purchaseOrder);

                if (purchaseOrder.Status == PurchaseOrderStatus.OperativePending 
                    && CanAddOperationPurchaseOrder(purchaseOrder, operUserIds))
                    result.Add(purchaseOrder);

                if (purchaseOrder.Status == PurchaseOrderStatus.DafPending 
                    && isDaf)
                    result.Add(purchaseOrder);
            }

            return new Response<IList<PurchaseOrderPendingModel>>
            {
                Data = Translate(result)
            };
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.FindLite(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            PurchaseOrderValidationHelper.Delete(response, purchaseOrder, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.PurchaseOrderRepository.Delete(purchaseOrder);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.DeleteSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private PurchaseOrderHistory GetHistory(PurchaseOrderDomain purchaseOrder, PurchaseOrderStatusParams model)
        {
            var history = new PurchaseOrderHistory
            {
                From = purchaseOrder.Status,
                PurchaseOrderId = purchaseOrder.Id,
                UserId = userData.GetCurrentUser().Id,
                CreatedDate = DateTime.UtcNow,
                Comment = model.Comments
            };

            return history;
        }

        public Response<PurchaseOrderDomain> GetById(int id)
        {
            var response = new Response<PurchaseOrderDomain>();

            response.Data = PurchaseOrderValidationHelper.FindWithAnalytic(id, response, unitOfWork);

            return response;
        }

        public IList<PurchaseOrderDomain> GetByService(string serviceId)
        {
            return unitOfWork.PurchaseOrderRepository.GetByService(serviceId);
        }

        public IList<PurchaseOrderDomain> GetByServiceLite(string serviceId, string opportunityNumber)
        {
            var list = unitOfWork.PurchaseOrderRepository.GetByServiceLite(serviceId);

            if (opportunityNumber.Equals("0"))
            {
                return list.ToList();
            }
            else
            {
                return list.Where(x => !string.IsNullOrWhiteSpace(x.Proposal) && x.Proposal.Contains(opportunityNumber)).ToList();
            }
        }

        private void Validate(PurchaseOrderModel model, Response<PurchaseOrderDomain> response)
        {
            PurchaseOrderValidationHelper.ValidateNumber(response, model, unitOfWork);
            PurchaseOrderValidationHelper.ValidateAnalytic(response, model);
            PurchaseOrderValidationHelper.ValidateOpportunities(response, model);
            PurchaseOrderValidationHelper.ValidateClient(response, model);
            PurchaseOrderValidationHelper.ValidateArea(response, model);
            PurchaseOrderValidationHelper.ValidateCurrency(response, model);
            PurchaseOrderValidationHelper.ValidateDates(response, model);
            PurchaseOrderValidationHelper.ValidateMargin(response, model);
            PurchaseOrderValidationHelper.ValidateAmmount(response, model.AmmountDetails);
        }

        private List<PurchaseOrderPendingModel> Translate(List<PurchaseOrderDomain> purchaseOrders)
        {
            return mapper.Map<List<PurchaseOrderDomain>, List<PurchaseOrderPendingModel>>(purchaseOrders);
        }

        private bool IsCompliance(UserLiteModel currentUser)
        {
            var isCompliance = unitOfWork.UserRepository.HasComplianceGroup(currentUser.Email);

            if (!isCompliance)
            {
                isCompliance =
                    unitOfWork.UserDelegateRepository.GetByUserId(currentUser.Id,
                        UserDelegateType.PurchaseOrderApprovalCompliance).Any();
            }

            return isCompliance;
        }

        private bool IsDaf(UserLiteModel currentUser)
        {
            var isDaf = unitOfWork.UserRepository.HasDafPurchaseOrderGroup(currentUser.Email);

            if (!isDaf)
            {
                isDaf =
                    unitOfWork.UserDelegateRepository.GetByUserId(currentUser.Id,
                        UserDelegateType.PurchaseOrderApprovalDaf).Any();
            }

            return isDaf;
        }

        private List<int> GetCommercialUsers(UserLiteModel currentUser)
        {
            var result = new List<int> {currentUser.Id};

            var userDelegates = unitOfWork.UserDelegateRepository.GetByUserId(currentUser.Id,
                UserDelegateType.PurchaseOrderApprovalCommercial);
            if (userDelegates.Any(s => s.SourceId != null))
            {
                result.AddRange(userDelegates.Select(s => s.SourceId.Value));
            }

            return result;
        }

        private List<int> GetOperationUsers(UserLiteModel currentUser)
        {
            var result = new List<int> {currentUser.Id};

            var userDelegates = unitOfWork.UserDelegateRepository.GetByUserId(currentUser.Id,
                UserDelegateType.PurchaseOrderApprovalOperation);
            if (userDelegates.Any(s => s.SourceId != null))
            {
                result.AddRange(userDelegates.Select(s => s.SourceId.Value));
            }

            return result;
        }

        private bool CanAddOperationPurchaseOrder(PurchaseOrderDomain purchaseOrder, List<int> userIds)
        {
            return purchaseOrder.PurchaseOrderAnalytics
                    .Where(s => s.Analytic != null)
                    .Where(s => s.Analytic.Sector != null)
                    .Any(s => userIds.Contains(s.Analytic.Sector.ResponsableUserId));
        }
    }
}