using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacViewModel
    {
        public SolfacViewModel()
        {
            Hitos = new List<HitoViewModel>();
            Details = new List<HitoDetailViewModel>();
            InvoicesId = new List<int>();
        }

        public int Id { get; set; }

        public string ClientName { get; set; }

        public string BusinessName { get; set; }

        public string CelPhone { get; set; }

        public SolfacStatus StatusId { get; set; }

        public string StatusName { get; set; }

        public string ContractNumber { get; set; }

        public string Project { get; set; }

        public string ProjectId { get; set; }

        [Required(ErrorMessage = "billing/solfac.documentTypeRequired")]
        public int? DocumentType { get; set; }

        [Required(ErrorMessage = "billing/solfac.userApplicantIdRequired")]
        public int? UserApplicantId { get; set; }

        public string ImputationNumber1 { get; set; }

        public string ImputationNumber2 { get; set; }

        [Required(ErrorMessage = "billing/solfac.imputationNumber3Required")]
        public int? ImputationNumber3 { get; set; }

        public decimal Amount { get; set; }

        public decimal Iva21 { get; set; }

        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "billing/solfac.currencyRequired")]
        public int? CurrencyId { get; set; }

        public decimal? CapitalPercentage { get; set; }

        public decimal? BuenosAiresPercentage { get; set; }

        public decimal? OtherProvince1Percentage { get; set; }

        public decimal? OtherProvince2Percentage { get; set; }

        public decimal? OtherProvince3Percentage { get; set; }

        public int Province1Id { get; set; }

        public int Province2Id { get; set; }

        public int Province3Id { get; set; }

        public string AttachedParts { get; set; }

        public string ParticularSteps { get; set; }

        public int PaymentTermId { get; set; }

        public ICollection<HitoViewModel> Hitos { get; set; }

        public ICollection<HitoDetailViewModel> Details { get; set; }

        public DateTime StartDate { get; set; }

        public int ModifiedByUserId { get; set; }

        public DateTime UpdatedDate { get; set; }

        public IList<int> InvoicesId { get; set; }

        public string CustomerId { get; set; }

        public string ServiceId { get; set; }

        public string Service { get; set; }

        public string Analytic { get; set; }

        public bool IsNew { get; set; }

        public bool WithTax { get; set; }

        public bool Remito { get; set; }

        public Solfac CreateDomain()
        {
            var solfac = new Solfac();

            solfac.ClientName = ClientName;
            solfac.BusinessName = BusinessName;
            solfac.CelPhone = CelPhone;
            solfac.Status = StatusId;
            solfac.ContractNumber = ContractNumber;
            solfac.Project = Project;
            solfac.DocumentTypeId = DocumentType.GetValueOrDefault();
            solfac.UserApplicantId = UserApplicantId.GetValueOrDefault();
            solfac.ImputationNumber1 = ImputationNumber1;
            solfac.ImputationNumber3Id = ImputationNumber3.GetValueOrDefault();
            solfac.TotalAmount = TotalAmount;
            solfac.CurrencyId = CurrencyId.GetValueOrDefault();
            solfac.CapitalPercentage = CapitalPercentage.GetValueOrDefault();
            solfac.BuenosAiresPercentage = BuenosAiresPercentage.GetValueOrDefault();
            solfac.OtherProvince1Percentage = OtherProvince1Percentage.GetValueOrDefault();
            solfac.OtherProvince2Percentage = OtherProvince2Percentage.GetValueOrDefault();
            solfac.OtherProvince3Percentage = OtherProvince3Percentage.GetValueOrDefault();
            solfac.Province1Id = Province1Id;
            solfac.Province2Id = Province2Id;
            solfac.Province3Id = Province3Id;
            solfac.ParticularSteps = ParticularSteps;
            solfac.PaymentTermId = PaymentTermId;
            solfac.ProjectId = ProjectId;
            solfac.ServiceId = ServiceId;
            solfac.CustomerId = CustomerId;
            solfac.Analytic = Analytic;
            solfac.Service = Service;
            solfac.WithTax = WithTax;
            solfac.InvoiceRequired = Remito;

            foreach (var hitoViewModel in Hitos)
            {
                var hito = hitoViewModel.CreateDomain();

                var details = Details.Where(x => x.ExternalHitoId == hitoViewModel.ExternalHitoId);

                hito.Details.AddRange(details.Select(x => x.CreateDomain()));

                solfac.Hitos.Add(hito);
            }
             
            return solfac;
        }
    }
}
