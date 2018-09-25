using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Domain.Models.Admin;
using Sofco.Domain.Relationships;

namespace Sofco.Domain.Models.Billing
{
    public class Solfac : BaseEntity
    {
        public Solfac()
        {
            Hitos = new Collection<Hito>();
            Histories = new Collection<SolfacHistory>();
            StartDate = DateTime.Now;
            Invoices = new Collection<Invoice>();
        }
        
        public string BusinessName { get; set; }
        public string CelPhone { get; set; }
        public SolfacStatus Status { get; set; }

        public string InvoiceCode { get; set; }

        public decimal TotalAmount { get; set; }

        public bool WithTax { get; set; }

        public decimal CapitalPercentage { get; set; }
        public decimal BuenosAiresPercentage { get; set; }
        public decimal OtherProvince1Percentage { get; set; }
        public decimal OtherProvince2Percentage { get; set; }
        public decimal OtherProvince3Percentage { get; set; }
        public int Province1Id { get; set; }
        public int Province2Id { get; set; }
        public int Province3Id { get; set; }
        public string ParticularSteps { get; set; }

        public string Analytic { get; set; }

        public ICollection<Hito> Hitos { get; set; }
        public ICollection<SolfacHistory> Histories { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }

        public int UserApplicantId { get; set; }
        public User UserApplicant { get; set; }

        public string ImputationNumber1 { get; set; }
        public ImputationNumber ImputationNumber { get; set; }
        public int ImputationNumber3Id { get; set; }

        public int PaymentTermId { get; set; }
        public PaymentTerm PaymentTerm { get; set; }

        public DateTime StartDate { get; set; }
        public int ModifiedByUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? CashedDate { get; set; }

        public string ProjectId { get; set; }
        public string Project { get; set; }

        public string CustomerId { get; set; }
        public string ClientName { get; set; }

        public string ServiceId { get; set; }
        public string Service { get; set; }

        public ICollection<Invoice> Invoices { get; set; }
        public ICollection<SolfacAttachment> Attachments { get; set; }

        public bool InvoiceRequired { get; set; }

        public string Integrator { get; set; }

        public string IntegratorId { get; set; }

        public string Manager { get; set; }

        public string ManagerId { get; set; }

        public ICollection<SolfacCertificate> SolfacCertificates { get; set; }

        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        public string OpportunityNumber { get; set; }
    }
}
