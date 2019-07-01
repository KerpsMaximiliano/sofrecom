using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing.PurchaseOrder;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.Billing;

namespace Sofco.Service.Implementations.Billing.PurchaseOrder
{
    public class PurchaseOrderStatusService : IPurchaseOrderStatusService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<PurchaseOrderStatusService> logger;
        private readonly IUserData userData;
        private readonly IPurchaseOrderStatusFactory purchaseOrderStatusFactory;

        public PurchaseOrderStatusService(IUnitOfWork unitOfWork,
            ILogMailer<PurchaseOrderStatusService> logger,
            IPurchaseOrderStatusFactory purchaseOrderStatusFactory,
            IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            this.purchaseOrderStatusFactory = purchaseOrderStatusFactory;
        }

        public Response ChangeStatus(int id, PurchaseOrderStatusParams model)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.FindLite(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            var statusHandler = purchaseOrderStatusFactory.GetInstance(purchaseOrder.Status);

            try
            {
                // Validate Status
                statusHandler.Validate(response, model, purchaseOrder);

                if (response.HasErrors()) return response;

                var history = GetHistory(purchaseOrder, model);

                // Update Status
                statusHandler.Save(purchaseOrder, model);

                // Add History
                history.To = purchaseOrder.Status;
                unitOfWork.PurchaseOrderRepository.AddHistory(history);

                // Save
                unitOfWork.Save();
                response.AddSuccess(statusHandler.GetSuccessMessage(model));
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Billing.PurchaseOrder.CannotChangeStatus);
            }

            try
            {
                if (response.HasErrors()) return response;
                statusHandler.SendMail(purchaseOrder, model);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddWarning(Resources.Common.ErrorSendMail);
            }

            return response;
        }

        public Response MakeAdjustment(int id, PurchaseOrderAdjustmentModel model)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.Find(id, response, unitOfWork);
            PurchaseOrderValidationHelper.ValidateAdjustmentAmmount(response, model.Items);

            if (response.HasErrors()) return response;

            try
            {
                var history = GetHistory(purchaseOrder, new PurchaseOrderStatusParams());

                purchaseOrder.Status = PurchaseOrderStatus.Closed;
                purchaseOrder.Adjustment = true; 
                purchaseOrder.AdjustmentDate = DateTime.UtcNow;
                purchaseOrder.CommentsForAdjustment = model.Comments;
                unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
                unitOfWork.PurchaseOrderRepository.UpdateAdjustment(purchaseOrder);

                foreach (var detail in purchaseOrder.AmmountDetails)
                {
                    var modelDetail = model.Items.SingleOrDefault(x => x.CurrencyId == detail.CurrencyId);

                    if (modelDetail == null) continue;

                    detail.Adjustment = modelDetail.Adjustment;
                    unitOfWork.PurchaseOrderRepository.UpdateDetail(detail);
                }

                // Add History
                history.To = purchaseOrder.Status;
                unitOfWork.PurchaseOrderRepository.AddHistory(history);

                unitOfWork.Save();
                response.AddSuccess(Resources.Billing.PurchaseOrder.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Reopen(int id, PurchaseOrderStatusParams model)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.FindLite(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            PurchaseOrderValidationHelper.CanReopen(response, purchaseOrder);

            if (response.HasErrors()) return response;

            try
            {
                var history = GetHistory(purchaseOrder, model);

                purchaseOrder.Status = PurchaseOrderStatus.Valid;
                unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);

                // Add History
                history.To = purchaseOrder.Status;
                unitOfWork.PurchaseOrderRepository.AddHistory(history);

                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.CloseSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Close(int id, PurchaseOrderStatusParams model)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.FindLite(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            PurchaseOrderValidationHelper.Close(response, purchaseOrder);

            if (response.HasErrors()) return response;

            try
            {
                var history = GetHistory(purchaseOrder, model);

                purchaseOrder.Status = PurchaseOrderStatus.Closed;
                unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);

                // Add History
                history.To = purchaseOrder.Status;
                unitOfWork.PurchaseOrderRepository.AddHistory(history);

                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.CloseSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private PurchaseOrderHistory GetHistory(Domain.Models.Billing.PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
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
    }
}
