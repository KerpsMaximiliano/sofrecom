using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.WebApi.Migrations;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacDetail
    {
        public SolfacDetail()
        {
            
        }

        public SolfacDetail(Model.Models.Billing.Solfac domain)
        {
            Id = domain.Id;
            ClientName = domain.ClientName;
            BusinessName = domain.BusinessName;
            CelPhone = domain.CelPhone;
            StatusName = domain.Status.ToString();
            StatusId = domain.Status;
            Project = domain.Project;
            ProjectId = domain.ProjectId;
            ImputationNumber1 = domain.ImputationNumber1;
            Amount = domain.Amount;
            Iva21 = domain.Iva21;
            TotalAmount = domain.TotalAmount;
            ParticularSteps = domain.ParticularSteps;
            TimeLimit = domain.TimeLimit;
            CurrencyId = domain.CurrencyId;
            UserApplicantId = domain.UserApplicantId;
            DocumentTypeId = domain.DocumentType.Id;
            ContractNumber = domain.ContractNumber;
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

            CustomerId = domain.CustomerId;
            ServiceId = domain.ServiceId;
            ServiceName = domain.Service;

            if (domain.ImputationNumber != null)
            {
                ImputationNumber3 = domain.ImputationNumber.Text;
                ImputationNumber3Id = domain.ImputationNumber.Id;
            }

            if (domain.UserApplicant != null)
                UserApplicantName = domain.UserApplicant.Name;
                
            if (domain.DocumentType != null)
                DocumentType = domain.DocumentType.Text;

            if(domain.Currency != null)
                CurrencyName = domain.Currency.Text;

            if (domain.Invoice != null)
            {
                InvoiceId = domain.Invoice.Id;
                InvoiceNumber = domain.Invoice.InvoiceNumber;
                PdfFileName = domain.Invoice.PdfFileName;
            }

            Hitos = new List<HitoViewModel>();

            foreach (var hito in domain.Hitos){
                Hitos.Add(new HitoViewModel(hito));
            }
        }

        public string ServiceName { get; set; }

        public int UserApplicantId { get; set; }
        public SolfacStatus StatusId { get; set; }
        public int DocumentTypeId { get; set; }
        public int ImputationNumber3Id { get; set; }
        public int Id { get; set; }
        public string PdfFileName { get; set; }
        public string InvoiceNumber { get; set; }
        public int InvoiceId { get; set; }
        public string ClientName { get; set; }
        public string BusinessName { get; set; }
        public string CelPhone { get; set; }
        public string StatusName { get; set; }
        public string ContractNumber { get; set; }
        public string DocumentType { get; set; }
        public string Project { get; set; }
        public string ProjectId { get; set; }
        public string ServiceId { get; set; }
        public string CustomerId { get; set; }
        public int CurrencyId { get; set; }
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
        public string AttachedParts { get; set; }
        public string ParticularSteps { get; set; }
        public short TimeLimit { get; set; }
        public string InvoiceCode { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? CashedDate { get; set; }

        public ICollection<HitoViewModel> Hitos { get; set; }
        public string ProvinceName1 { get; set; }
        public string ProvinceName2 { get; set; }
        public string ProvinceName3 { get; set; }

        public int Province1Id { get; set; }
        public int Province2Id { get; set; }
        public int Province3Id { get; set; }

        public string Comments { get; set; }

        public Model.Models.Billing.Solfac CreateDomain()
        {
            var solfac = new Model.Models.Billing.Solfac();

            solfac.Id = Id;
            solfac.ClientName = ClientName;
            solfac.BusinessName = BusinessName;
            solfac.CelPhone = CelPhone;
            solfac.Status = StatusId;
            solfac.ContractNumber = ContractNumber;
            solfac.Project = Project;
            solfac.DocumentTypeId = DocumentTypeId;
            solfac.UserApplicantId = UserApplicantId;
            solfac.ImputationNumber1 = ImputationNumber1;
            solfac.ImputationNumber3Id = ImputationNumber3Id;
            solfac.Amount = Amount;
            solfac.Iva21 = Iva21;
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
            solfac.TimeLimit = TimeLimit;
            solfac.ProjectId = ProjectId;
            solfac.ServiceId = ServiceId;
            solfac.CustomerId = CustomerId;
            solfac.Analytic = ImputationNumber1;
            solfac.Service = ServiceName;

            if (InvoiceId > 0)
                solfac.InvoiceId = InvoiceId;

            foreach (var hitoViewModel in Hitos)
                solfac.Hitos.Add(hitoViewModel.CreateDomain());

            return solfac;
        }
    }
}
