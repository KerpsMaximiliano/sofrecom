using Sofco.Core.DAL;
using Sofco.Model.Utils;
using PurchaseOrder = Sofco.Model.Models.Billing.PurchaseOrder;

namespace Sofco.Framework.ValidationHelpers.Billing
{
    public static class PurchaseOrderValidationHelper
    {
        public static void ValidateTitle(Response response, PurchaseOrder domain)
        {
            if (string.IsNullOrWhiteSpace(domain.Title))
            {
                response.AddError(Resources.Billing.PurchaseOrder.TitleIsRequired);
            }
        }

        public static void ValidateNumber(Response response, PurchaseOrder domain)
        {
            if (string.IsNullOrWhiteSpace(domain.Number))
            {
                response.AddError(Resources.Billing.PurchaseOrder.NumberIsRequired);
            }
        }

        public static void ValidateAnalytic(Response response, PurchaseOrder domain)
        {
            if (domain.AnalyticId == 0)
            {
                response.AddError(Resources.Billing.PurchaseOrder.AnalyticIsRequired);
            }
        }

        public static void ValidateClient(Response response, PurchaseOrder domain)
        {
            if (string.IsNullOrWhiteSpace(domain.ClientExternalId))
            {
                response.AddError(Resources.Billing.PurchaseOrder.ClientIsRequired);
            }
        }

        public static void ValidateComercialManager(Response response, PurchaseOrder domain)
        {
            if (domain.CommercialManagerId == 0)
            {
                response.AddError(Resources.Billing.PurchaseOrder.ComercialManagerIsRequired);
            }
        }

        public static void ValidateManager(Response response, PurchaseOrder domain)
        {
            if (domain.ManagerId == 0)
            {
                response.AddError(Resources.Billing.PurchaseOrder.ManagerIsRequired);
            }
        }

        public static void ValidateArea(Response response, PurchaseOrder domain)
        {
            if (string.IsNullOrWhiteSpace(domain.Area))
            {
                response.AddError(Resources.Billing.PurchaseOrder.AreaIsRequired);
            }
        }

        public static void ValidateYear(Response response, PurchaseOrder domain)
        {
            if (domain.Year >= 2015 && domain.Year < 2099)
            {
                response.AddError(Resources.Billing.PurchaseOrder.YearIsRequired);
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

        public static void Exist(Response<PurchaseOrder> response, PurchaseOrder domain, IUnitOfWork unitOfWork)
        {
            var exist = unitOfWork.PurchaseOrderRepository.Exist(domain.Id);

            if (!exist)
            {
                response.AddError(Resources.Billing.PurchaseOrder.NotFound);
            }
        }
    }
}
