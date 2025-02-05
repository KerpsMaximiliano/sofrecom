﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Domain.Enums;
using Sofco.Domain.Helpers;

namespace Sofco.Core.Models.Billing
{
    public class SolfacDetail
    {
        public SolfacDetail()
        {
        }

        public SolfacDetail(Domain.Models.Billing.Solfac domain)
        {
            Id = domain.Id;
            ClientName = domain.AccountName;
            BusinessName = domain.BusinessName;
            CelPhone = domain.CelPhone;
            StatusName = domain.Status.ToString();
            StatusId = domain.Status;
            Project = domain.Project;
            ImputationNumber1 = domain.ImputationNumber1;
            TotalAmount = domain.TotalAmount;
            ParticularSteps = domain.ParticularSteps;
            PaymentTerm = domain.PaymentTerm; 
            CurrencyId = domain.CurrencyId;
            UserApplicantId = domain.UserApplicantId;
            DocumentTypeId = domain.DocumentType.Id;
            CapitalPercentage = domain.CapitalPercentage;
            BuenosAiresPercentage = domain.BuenosAiresPercentage;
            OtherProvince1Percentage = domain.OtherProvince1Percentage;
            OtherProvince2Percentage = domain.OtherProvince2Percentage;
            OtherProvince3Percentage = domain.OtherProvince3Percentage;
            Province1Id = domain.Province1Id;
            Province2Id = domain.Province2Id;
            Province3Id = domain.Province3Id;
            InvoiceCode = domain.InvoiceCode;
            InvoiceDate = domain.InvoiceDate;
            CashedDate = domain.CashedDate;
            WithTax = domain.WithTax;
            InvoiceRequired = domain.InvoiceRequired;
            OpportunityNumber = domain.OpportunityNumber;
            CurrencyExchange = domain.CurrencyExchange;
            IdToCompareByCreditNote = domain.IdToCompareByCreditNote;

            CustomerId = domain.AccountId;
            ServiceId = domain.ServiceId;
            ServiceName = domain.Service;
            Integrator = domain.Integrator;
            IntegratorId = domain.IntegratorId;
            ManagerId = domain.ManagerId;
            Manager = domain.Manager;
            PurchaseOrderId = domain.PurchaseOrderId;
            ContactEmail = domain.ContactEmail;

            if (domain.ProjectId.Contains(";"))
            {
                var split = domain.ProjectId.Split(';');
                ProjectId = split[0];
                IsMultiple = true;
            }
            else
            {
                ProjectId = domain.ProjectId;
                IsMultiple = false;
            }

            if (domain.ImputationNumber != null)
            {
                ImputationNumber3 = domain.ImputationNumber.Text;
                ImputationNumber3Id = domain.ImputationNumber.Id;
            }

            if (domain.UserApplicant != null)
                UserApplicantName = domain.UserApplicant.Name;

            if (domain.DocumentType != null)
                DocumentType = domain.DocumentType.Text;

            if (domain.Currency != null)
                CurrencyName = domain.Currency.Text;

            if (domain.PurchaseOrder != null)
                ContractNumber = domain.PurchaseOrder.Number;

            Hitos = new List<HitoModel>();
            Details = new List<HitoDetailModel>();

            foreach (var hito in domain.Hitos)
            {
                foreach (var hitoDetail in hito.Details)
                {
                    var detailViewModel = new HitoDetailModel(hitoDetail)
                    {
                        ExternalHitoId = hito.ExternalHitoId,
                    };
                    Details.Add(detailViewModel);
                }

                Hitos.Add(new HitoModel(hito));
            }

            if (SolfacHelper.IsCreditNote(domain))
            {
                IsCreditNote = true;
            }

            if (SolfacHelper.IsDebitNote(domain))
            {
                IsDebitNote = true;
            }
        }

        public string ContactEmail { get; set; }

        public int? IdToCompareByCreditNote { get; set; }

        public bool IsDebitNote { get; set; }

        public bool IsCreditNote { get; set; }

        public decimal? CurrencyExchange { get; set; }

        public string ServiceName { get; set; }

        public int UserApplicantId { get; set; }

        public SolfacStatus StatusId { get; set; }

        public int DocumentTypeId { get; set; }

        public int ImputationNumber3Id { get; set; }

        public int Id { get; set; }

        public string ClientName { get; set; }

        public string BusinessName { get; set; }

        public string CelPhone { get; set; }

        public string StatusName { get; set; }

        public int? PurchaseOrderId { get; set; }

        public string DocumentType { get; set; }

        public string Project { get; set; }

        public string ProjectId { get; set; }

        public string ServiceId { get; set; }

        public string CustomerId { get; set; }

        public int CurrencyId { get; set; }

        public string ContractNumber { get; set; }

        public string ImputationNumber1 { get; set; }

        public string ImputationNumber3 { get; set; }

        public string UserApplicantName { get; set; }

        public decimal Amount { get; set; }

        public decimal Iva21 { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal OtherProvince3Percentage { get; set; }

        public decimal OtherProvince2Percentage { get; set; }

        public decimal OtherProvince1Percentage { get; set; }

        public decimal BuenosAiresPercentage { get; set; }

        public decimal CapitalPercentage { get; set; }

        public string CurrencyName { get; set; }

        public string ParticularSteps { get; set; }

        public string InvoiceCode { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? CashedDate { get; set; }

        public ICollection<HitoModel> Hitos { get; set; }

        public ICollection<HitoDetailModel> Details { get; set; }

        public string ProvinceName1 { get; set; }

        public string ProvinceName2 { get; set; }

        public string ProvinceName3 { get; set; }

        public int Province1Id { get; set; }

        public int Province2Id { get; set; }

        public int Province3Id { get; set; }

        public string Comments { get; set; }

        public bool WithTax { get; set; }

        public bool InvoiceRequired { get; set; }

        public bool IsMultiple { get; set; }

        public string Integrator { get; set; }

        public string IntegratorId { get; set; }

        public string Manager { get; set; }

        public string ManagerId { get; set; }

        public string OpportunityNumber { get; set; }

        public string PaymentTerm { get; set; }

        public Domain.Models.Billing.Solfac CreateDomain()
        {
            var solfac = new Domain.Models.Billing.Solfac();

            solfac.Id = Id;
            solfac.AccountName = ClientName;
            solfac.BusinessName = BusinessName;
            solfac.CelPhone = CelPhone;
            solfac.Status = StatusId;
            solfac.PurchaseOrderId = PurchaseOrderId;
            solfac.Project = Project;
            solfac.DocumentTypeId = DocumentTypeId;
            solfac.UserApplicantId = UserApplicantId;
            solfac.ImputationNumber1 = ImputationNumber1;
            solfac.ImputationNumber3Id = ImputationNumber3Id;
            solfac.TotalAmount = TotalAmount;
            solfac.CurrencyId = CurrencyId;
            solfac.CapitalPercentage = CapitalPercentage;
            solfac.BuenosAiresPercentage = BuenosAiresPercentage;
            solfac.OtherProvince1Percentage = OtherProvince1Percentage;
            solfac.OtherProvince2Percentage = OtherProvince2Percentage;
            solfac.OtherProvince3Percentage = OtherProvince3Percentage;
            solfac.Province1Id = Province1Id;
            solfac.Province2Id = Province2Id;
            solfac.Province3Id = Province3Id;
            solfac.ParticularSteps = ParticularSteps;
            solfac.PaymentTerm = PaymentTerm;
            solfac.ProjectId = ProjectId;
            solfac.ServiceId = ServiceId;
            solfac.AccountId = CustomerId;
            solfac.Analytic = ImputationNumber1;
            solfac.Service = ServiceName;
            solfac.WithTax = WithTax;
            solfac.InvoiceRequired = InvoiceRequired;
            solfac.ManagerId = ManagerId;
            solfac.Manager = Manager;
            solfac.OpportunityNumber = OpportunityNumber;
            solfac.IdToCompareByCreditNote = IdToCompareByCreditNote;
            solfac.ContactEmail = ContactEmail;

            if (SolfacHelper.IsCreditNote(solfac) && solfac.TotalAmount > 0)
            {
                solfac.TotalAmount *= -1;
            }

            foreach (var hitoViewModel in Hitos)
            {
                var hito = hitoViewModel.CreateDomain();

                var details = Details.Where(x => x.ExternalHitoId == hitoViewModel.ExternalHitoId);

                foreach (var hitoDetailViewModel in details)
                {
                    var domainDetail = hitoDetailViewModel.CreateDomain();
                    domainDetail.HitoId = hito.Id;
                    hito.Details.Add(domainDetail);
                }

                hito.Total = hito.Details.Sum(x => x.Total);

                solfac.Hitos.Add(hito);
            }

            return solfac;
        }
    }
}
