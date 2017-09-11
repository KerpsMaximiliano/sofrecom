using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacDetail
    {
        public SolfacDetail(Solfac domain)
        {
            ClientName = domain.ClientName;
            BusinessName = domain.BusinessName;
            CelPhone = domain.CelPhone;
            StatusName = domain.Status.ToString();
            Project = domain.Project;
            ProjectId = domain.ProjectId;
            ImputationNumber1 = domain.ImputationNumber1;
            Amount = domain.Amount;
            Iva21 = domain.Iva21;
            TotalAmount = domain.TotalAmount;
            AttachedParts = domain.AttachedParts;
            ParticularSteps = domain.ParticularSteps;
            TimeLimit = domain.TimeLimit;
            CurrencyId = domain.CurrencyId;
            ContractNumber = domain.ContractNumber;
            CapitalPercentage = domain.CapitalPercentage;
            BuenosAiresPercentage = domain.BuenosAiresPercentage;
            OtherProvince1Percentage = domain.OtherProvince1Percentage;
            OtherProvince2Percentage = domain.OtherProvince2Percentage;
            OtherProvince3Percentage = domain.OtherProvince3Percentage;

            if (domain.ImputationNumber != null)
                ImputationNumber3 = domain.ImputationNumber.Text;

            if(domain.UserApplicant != null)
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

        public ICollection<HitoViewModel> Hitos { get; set; }
        public string ProvinceName1 { get; set; }
        public string ProvinceName2 { get; set; }
        public string ProvinceName3 { get; set; }
    }
}
