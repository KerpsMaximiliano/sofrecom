using System;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class SolfacSearchDetail
    {
        public SolfacSearchDetail(Solfac domain)
        {
            Id = domain.Id;
            Project = domain.Project;
            BusinessName = domain.BusinessName;

            if (domain.DocumentType != null)
            {
                DocumentTypeId = domain.DocumentTypeId;
                DocumentTypeName = domain.DocumentType.Text;
            }

            Manager = domain.Manager;

            StartDate = domain.StartDate;
            TotalAmount = domain.TotalAmount;
            StatusName = domain.Status.ToString();
            CurrencyId = domain.CurrencyId;

            InvoiceDate = domain.InvoiceDate;
            InvoiceCode = domain.InvoiceCode;

            CustomerId = domain.CustomerId;
            ServiceId = domain.ServiceId;
            ProjectId = domain.ProjectId;

            if (domain.PurchaseOrder != null)
                PurchaseOrder = domain.PurchaseOrder.Number;

            if (domain.ProjectId.Contains(";"))
            {
                var split = domain.ProjectId.Split(';');
                ProjectQuantity = split.Length;
            }
            else
            {
                ProjectQuantity = 1;
            }
        }

        public string ProjectId { get; set; }

        public string ServiceId { get; set; }

        public string CustomerId { get; set; }

        public int Id { get; set; }

        public int DocumentTypeId { get; set; }

        public string Project { get; set; }

        public string BusinessName { get; set; }

        public string PurchaseOrder { get; set; }

        public string DocumentTypeName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public decimal Amount { get; set; }

        public decimal Iva21 { get; set; }

        public decimal TotalAmount { get; set; }

        public string StatusName { get; set; }

        public int CurrencyId { get; set; }

        public string Manager { get; set; }

        public string InvoiceCode { get; set; }

        public int ProjectQuantity { get; set; }
    }
}
