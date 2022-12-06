using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Providers;
using Sofco.Domain.Utils;

namespace Sofco.Domain.Models.RequestNote
{
    public class BuyOrder : WorkflowEntity
    {
        public int RequestNoteId { get; set; }
        public RequestNote RequestNote { get; set; }
        public int ProviderId { get; set; }
        public RequestNoteProvider Provider { get; set; }
        public decimal? TotalAmmount { get; set; }
        public string BuyOrderNumber { get; set; }
        public DateTime CreationDate { get; set; }
        public int CreationUserId { get; set; }
        public Admin.User CreationUser { get; set; }
        public int WorkflowId { get; set; }
        public Workflow.Workflow Workflow { get; set; }
        public IList<BuyOrderProductService> ProductsServices { get; set; }
        public IList<BuyOrderInvoice> Invoices { get; set; }
        public IList<BuyOrderHistory> Histories { get; set; }
    }
}
