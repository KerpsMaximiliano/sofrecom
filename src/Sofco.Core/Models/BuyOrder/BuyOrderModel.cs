using Sofco.Common.Settings;
using Sofco.Domain.Models.RequestNote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Core.Models.BuyOrder
{
    public class BuyOrderModel
    {

        private readonly AppSetting _settings;
        public int Id { get; set; }

        public int RequestNoteId { get; set; }

        public int? UserApplicantId { get; set; }

        public string Number { get; set; }

        public int ProviderId { get; set; }

        public string ProviderDescription { get; set; }

        public decimal? TotalAmount { get; set; }

        public int WorkflowId { get; set; }

        public int StatusId { get; set; }

        public string StatusDescription { get; set; }

        public bool HasEditPermissions { get; set; }
        public bool HasReadPermissions { get; set; }

        public IList<BuyOrderDetailModel> Items { get; set; }
        public BuyOrderInvoiceModel Invoice { get; set; }
        private bool ValidateReadPermissions(List<string> permissions, int userId)
        {
            return permissions.Any(p => p == "OC_READONLY");
        }
        private bool ValidateEditPermissions(List<string> permissions, int userId)
        {
            var hasPermission = false;
            if (StatusId == _settings.WorkflowStatusBOPendienteAprobacionDAF)
                hasPermission = permissions.Any(p => p == "OC_PEND_APR_DAF");
            else if (StatusId == _settings.WorkflowStatusBOPendienteRecepcionMerc)
                hasPermission = permissions.Any(p => p == "OC_PEND_REC_MERC");
            else if (StatusId == _settings.WorkflowStatusBOPendienteRecepcionFact)
                hasPermission = permissions.Any(p => p == "OC_PEND_REC_FACT");
            return hasPermission;
        }
        public BuyOrderModel()
        {

        }

        public BuyOrderModel(Domain.Models.RequestNote.BuyOrder order, List<string> permissions, int userId, AppSetting settings)
        {
            _settings = settings;
            Id = order.Id;
            RequestNoteId = order.RequestNoteId;
            Number = order.BuyOrderNumber;
            ProviderId = order.ProviderId;
            ProviderDescription = order.Provider?.Name;
            HasReadPermissions = ValidateReadPermissions(permissions, userId);
            HasEditPermissions = ValidateEditPermissions(permissions, userId);
            TotalAmount = order.TotalAmmount;
            WorkflowId = order.WorkflowId;
            StatusId = order.StatusId;
            StatusDescription = order.Status?.Name;
            if (order.ProductsServices != null)
                Items = order.ProductsServices.Select(p => new BuyOrderDetailModel()
                {
                    Id = p.Id,
                    Amount = p.Price,
                    Quantity = p.Quantity,
                    DeliveredQuantity = p.DeliveredQuantity,
                    Description = p.RequestNoteProductService?.ProductService,
                    RequestNoteProductServiceId = p.RequestNoteProductServiceId
                }).ToList();
            if(order.Invoices!= null && order.Invoices.Any())
            {
                Invoice = new BuyOrderInvoiceModel(order.Invoices.First());
            }
        }
        public Domain.Models.RequestNote.BuyOrder CreateDomain()
        {
            var domain = new Domain.Models.RequestNote.BuyOrder();

            domain.BuyOrderNumber = Number;
            domain.ProviderId = ProviderId;
            domain.TotalAmmount = TotalAmount;
            domain.UserApplicantId = UserApplicantId.GetValueOrDefault();
            domain.CreationUserId = UserApplicantId.GetValueOrDefault();
            domain.RequestNoteId = RequestNoteId;
            domain.InWorkflowProcess = true;
            domain.CreationDate = DateTime.UtcNow;
            domain.ProductsServices = new List<BuyOrderProductService>();
            foreach (var detail in Items)
            {
                domain.ProductsServices.Add(detail.CreateDomain());
            }
            return domain;
        }
    }
    public class BuyOrderDetailModel
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int Quantity { get; set; }

        public int? DeliveredQuantity { get; set; }

        public int? RequestedQuantity { get; set; }

        public int? PendingQuantity { get; set; }

        public int RequestNoteProductServiceId { get; set; }

        public BuyOrderProductService CreateDomain()
        {
            var domain = new BuyOrderProductService();
            domain.Price = Amount;
            domain.Quantity = Quantity;
            domain.DeliveredQuantity = DeliveredQuantity;
            domain.RequestNoteProductServiceId = RequestNoteProductServiceId;

            return domain;
        }
    }

    public class BuyOrderInvoiceModel
    {


        public int Id { get; set; }

        public int BuyOrderId { get; set; }

        public DateTime Date { get; set; }

        public string Number { get; set; }

        public string TaxCode { get; set; }

        public int? FileId { get; set; }
        public string FileDescription { get; set; }

        public IList<BuyOrderInvoiceDetailModel> Items { get; set; }

        public BuyOrderInvoiceModel()
        {

        }

        public BuyOrderInvoiceModel(Domain.Models.RequestNote.BuyOrderInvoice invoice)
        {
            Id = invoice.Id;
            BuyOrderId = invoice.BuyOrderId;
            Number = invoice.InvoiceNumber;
            TaxCode = invoice.TaxCode;
            Date = invoice.InvoiceDate;
            FileId = invoice.FileId;
            FileDescription = invoice.File?.FileName;
            if (invoice.ProductsServices != null)
                Items = invoice.ProductsServices.Select(p => new BuyOrderInvoiceDetailModel()
                {
                    Id = p.Id,
                    Amount = p.Price,
                    Quantity = p.Quantity,
                    BuyOrderProductServiceId = p.BuyOrderProductServiceId
                }).ToList();
        }
        public Domain.Models.RequestNote.BuyOrderInvoice CreateDomain()
        {
            var domain = new Domain.Models.RequestNote.BuyOrderInvoice();

            domain.InvoiceNumber = Number;
            domain.TaxCode = TaxCode;
            domain.InvoiceDate = Date;
            domain.BuyOrderId = BuyOrderId;
            domain.FileId = FileId;
            domain.ProductsServices = new List<BuyOrderInvoiceProductService>();
            foreach (var detail in Items)
            {
                domain.ProductsServices.Add(detail.CreateDomain());
            }
            return domain;
        }
    }
    public class BuyOrderInvoiceDetailModel
    {
        public int Id { get; set; }

        //public string Description { get; set; }

        public decimal Amount { get; set; }

        public int Quantity { get; set; }

        public int BuyOrderProductServiceId { get; set; }

        public BuyOrderInvoiceProductService CreateDomain()
        {
            var domain = new BuyOrderInvoiceProductService();
            domain.Price = Amount;
            domain.Quantity = Quantity;
            domain.BuyOrderProductServiceId = BuyOrderProductServiceId;

            return domain;
        }
    }
}
