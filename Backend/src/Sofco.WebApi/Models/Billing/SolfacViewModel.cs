using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacViewModel
    {
        public SolfacViewModel()
        {
            Hitos = new List<HitoViewModel>();
        }

        #region properties

        public int Id { get; set; }
        public string ClientName { get; set; }
        public string BusinessName { get; set; }
        public string CelPhone { get; set; }
        public SolfacStatus StatusId { get; set; }
        public string StatusName { get; set; }
        public string ContractNumber { get; set; }
        public string Project { get; set; }
        public string ProjectId { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        public int? DocumentTypeId { get; set; }
        [Required(ErrorMessage = "El solicitante es requerido")]
        public int? UserApplicantId { get; set; }

        public string ImputationNumber1 { get; set; }
        public string ImputationNumber2 { get; set; }

        [Required(ErrorMessage = "El número de imputacion es requerido")]
        public int? ImputationNumber3 { get; set; }

        public decimal Amount { get; set; }
        public decimal Iva21 { get; set; }
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "El la moneda es requerida")]
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
        public short TimeLimit { get; set; }

        public ICollection<HitoViewModel> Hitos { get; set; }

        public DateTime StartDate { get; set; }
        public int ModifiedByUserId { get; set; }
        public DateTime UpdatedDate { get; set; }

        #endregion

        public Solfac CreateDomain()
        {
            var solfac = new Solfac();

            solfac.ClientName = ClientName;
            solfac.BusinessName = BusinessName;
            solfac.CelPhone = CelPhone;
            solfac.Status = StatusId;
            solfac.ContractNumber = ContractNumber;
            solfac.Project = Project;
            solfac.DocumentTypeId = DocumentTypeId.GetValueOrDefault();
            solfac.UserApplicantId = UserApplicantId.GetValueOrDefault();
            solfac.ImputationNumber1 = ImputationNumber1;
            solfac.ImputationNumber2 = ImputationNumber2;
            solfac.ImputationNumber3Id = ImputationNumber3.GetValueOrDefault();
            solfac.Amount = Amount;
            solfac.Iva21 = Iva21;
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
            solfac.AttachedParts = AttachedParts;
            solfac.ParticularSteps = ParticularSteps;
            solfac.TimeLimit = TimeLimit;
            solfac.ProjectId = ProjectId;

            foreach (var hitoViewModel in Hitos)
                solfac.Hitos.Add(hitoViewModel.CreateDomain());

            return solfac;
        }
    }
}
