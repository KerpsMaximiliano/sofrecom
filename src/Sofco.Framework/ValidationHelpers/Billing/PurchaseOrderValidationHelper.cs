﻿using System;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.Billing;
using Sofco.Model.Utils;
using PurchaseOrder = Sofco.Model.Models.Billing.PurchaseOrder;

namespace Sofco.Framework.ValidationHelpers.Billing
{
    public static class PurchaseOrderValidationHelper
    {
        public static void ValidateNumber(Response response, PurchaseOrderModel domain)
        {
            if (string.IsNullOrWhiteSpace(domain.Number))
            {
                response.AddError(Resources.Billing.PurchaseOrder.NumberIsRequired);
            }
        }

        public static void ValidateAnalytic(Response response, PurchaseOrderModel domain)
        {
            if (!domain.AnalyticIds.Any())
            {
                response.AddError(Resources.Billing.PurchaseOrder.AnalyticIsRequired);
            }
        }

        public static void ValidateCurrency(Response response, PurchaseOrderModel domain)
        {
            if (domain.CurrencyId == 0)
            {
                response.AddError(Resources.Billing.PurchaseOrder.CurrencyIsRequired);
            }
        }

        public static void ValidateClient(Response response, PurchaseOrderModel domain)
        {
            if (string.IsNullOrWhiteSpace(domain.ClientExternalId))
            {
                response.AddError(Resources.Billing.PurchaseOrder.ClientIsRequired);
            }
        }

        public static void ValidateArea(Response response, PurchaseOrderModel domain)
        {
            if (string.IsNullOrWhiteSpace(domain.Area))
            {
                response.AddError(Resources.Billing.PurchaseOrder.AreaIsRequired);
            }
        }

        public static PurchaseOrder Find(int purchaseOrderId, Response response, IUnitOfWork unitOfWork)
        {
            var purchaseOrder = unitOfWork.PurchaseOrderRepository.GetById(purchaseOrderId);

            if (purchaseOrder == null)
            {
                response.AddError(Resources.Billing.PurchaseOrder.NotFound);
            }

            return purchaseOrder;
        }

        public static PurchaseOrder FindWithAnalytic(int purchaseOrderId, Response response, IUnitOfWork unitOfWork)
        {
            var purchaseOrder = unitOfWork.PurchaseOrderRepository.GetWithAnalyticsById(purchaseOrderId);

            if (purchaseOrder == null)
            {
                response.AddError(Resources.Billing.PurchaseOrder.NotFound);
            }

            return purchaseOrder;
        }

        public static void Exist(Response<PurchaseOrder> response, PurchaseOrderModel model, IUnitOfWork unitOfWork)
        {
            var exist = unitOfWork.PurchaseOrderRepository.Exist(model.Id);

            if (!exist)
            {
                response.AddError(Resources.Billing.PurchaseOrder.NotFound);
            }
        }

        public static void ValidateDates(Response response, PurchaseOrderModel domain)
        {
            if (domain.StartDate == DateTime.MinValue || domain.EndDate == DateTime.MinValue)
            {
                response.AddError(Resources.Billing.PurchaseOrder.DatesRequired);
            }
            else
            {
                if (domain.StartDate.Date > domain.EndDate.Date)
                {
                    response.AddError(Resources.Billing.PurchaseOrder.EndDateLessThanStartDate);
                }

            }
        }
    }
}
