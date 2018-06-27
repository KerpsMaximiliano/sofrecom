using System;
using System.Collections.Generic;
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
            if (domain.AmmountDetails.All(x => !x.Enable))
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
            if (domain.AreaId == 0)
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

        public static void Exist(Response response, int id, IUnitOfWork unitOfWork)
        {
            var exist = unitOfWork.PurchaseOrderRepository.Exist(id);

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

        public static void ValidateAmmount(Response response, IList<PurchaseOrderAmmountDetailModel> model)
        {
            if (model.Any(x => x.Enable && (x.Ammount < 0 || x.Ammount > 99999999)))
            {
                response.AddError(Resources.Billing.PurchaseOrder.AmmountRequired);
            }
        }

        public static void ValidateAdjustmentAmmount(Response response, IList<PurchaseOrderAmmountDetailModel> details)
        {
            if (details.Any(x => x.Adjustment < -99999999 || x.Adjustment == 0 || x.Adjustment > 99999999))
            {
                response.AddError(Resources.Billing.PurchaseOrder.AmmountRequired);
            }
        }
    }
}
