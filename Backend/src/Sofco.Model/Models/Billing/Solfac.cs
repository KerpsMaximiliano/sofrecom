using System;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sofco.Model.Models.Admin;

namespace Sofco.Model.Models.Billing
{
    public class Solfac
    {
        public Solfac()
        {
            Hitos = new Collection<Hito>();
            StartDate = DateTime.Now;
        }

        public int Id { get; set; }
        public string ClientName { get; set; }
        public string BusinessName { get; set; }
        public string CelPhone { get; set; }
        public SolfacStatus Status { get; set; }
        public string ContractNumber { get; set; }
        public string Project { get; set; }
        public DocumentType DocumentType { get; set; }
        public int DocumentTypeId { get; set; }
        public int UserApplicantId { get; set; }
        public User UserApplicant { get; set; }
        public string ImputationNumber1 { get; set; }
        public string ImputationNumber2 { get; set; }
        public int ImputationNumber3Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Iva21 { get; set; }
        public decimal TotalAmount { get; set; }
        public int CurrencyId { get; set; }
        public decimal CapitalPercentage { get; set; }
        public decimal BuenosAiresPercentage { get; set; }
        public decimal OtherProvince1Percentage { get; set; }
        public decimal OtherProvince2Percentage { get; set; }
        public decimal OtherProvince3Percentage { get; set; }
        public int Province1Id { get; set; }
        public int Province2Id { get; set; }
        public int Province3Id { get; set; }
        public string AttachedParts { get; set; }
        public string ParticularSteps { get; set; }
        public short TimeLimit { get; set; }
        public IList<Hito> Hitos { get; set; }

        public DateTime StartDate { get; set; }
        public int ModifiedByUserId { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
