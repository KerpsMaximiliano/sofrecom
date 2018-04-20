using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.Billing
{
    public class SolfacValidationHelper
    {
        public static void ValidateCasheDate(SolfacStatusParams parameters, Response response, Solfac solfac)
        {
            if (!parameters.CashedDate.HasValue)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CashedDateRequired, MessageType.Error));
            }
            else
            {
                if(parameters.CashedDate.Value.Date < solfac.InvoiceDate.Value.Date)
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.CashedDateGreaterThanInvoiceDate, MessageType.Error));
                }

                if (parameters.CashedDate.Value.Date < solfac.StartDate.Date)
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.CashedDateLessThanSolfacStartDate, MessageType.Error));
                }

                if (parameters.CashedDate.Value.Date > DateTime.Today.Date)
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.CashedDateGreaterThanToday, MessageType.Error));
                }
            }
        }

        public static Solfac ValidateIfExistAndGetWithUser(int id, ISolfacRepository solfacRepository, Response response)
        {
            var solfac = solfacRepository.GetByIdWithUser(id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
            }

            return solfac;
        }

        public static Solfac ValidateIfExist(int id, ISolfacRepository solfacRepository, Response response)
        {
            var solfac = solfacRepository.GetSingle(x => x.Id == id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.NotFound, MessageType.Error));
            }

            return solfac;
        }

        public static void ValidateInvoiceDate(SolfacStatusParams parameters, Response response, Solfac solfac)
        {
            if (!parameters.InvoiceDate.HasValue)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.InvoiceDateRequired, MessageType.Error));
            }
            else
            {
                if (parameters.InvoiceDate.Value.Date > DateTime.Today.Date)
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.InvoiceDateGreaterThanToday, MessageType.Error));
                }

                if (parameters.InvoiceDate.Value.Date < solfac.StartDate.Date)
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.InvoiceDateLessThanSolfacStartDate, MessageType.Error));
                }

                if (solfac.CashedDate.HasValue && solfac.CashedDate.Value.Date < parameters.InvoiceDate.Value.Date)
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.InvoiceDateGreaterThanCashedDate, MessageType.Error));
                }
            }
        }

        public static void ValidateComments(SolfacStatusParams parameters, Response response)
        {
            if (string.IsNullOrWhiteSpace(parameters.Comment))
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CommentRequired, MessageType.Error));
            }
        }

        public static void ValidateInvoiceCode(SolfacStatusParams parameters, ISolfacRepository solfacRepository, Response response, string invoiceCode)
        {
            if (string.IsNullOrWhiteSpace(parameters.InvoiceCode))
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.InvoiceCodeRequired, MessageType.Error));
            }
            else
            {
                if (parameters.InvoiceCode != invoiceCode && solfacRepository.InvoiceCodeExist(parameters.InvoiceCode))
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.InvoiceCodeAlreadyExist, MessageType.Error));
                }
            }
        }

        public static void ValidateProvincePercentage(Solfac solfac, Response response)
        {
            if (solfac.OtherProvince1Percentage > 0 && solfac.Province1Id == 0)
                response.Messages.Add(new Message(Resources.Billing.Solfac.ProvinceRequired, MessageType.Error));

            if (solfac.OtherProvince2Percentage > 0 && solfac.Province2Id == 0)
                response.Messages.Add(new Message(Resources.Billing.Solfac.ProvinceRequired, MessageType.Error));

            if (solfac.OtherProvince3Percentage > 0 && solfac.Province3Id == 0)
                response.Messages.Add(new Message(Resources.Billing.Solfac.ProvinceRequired, MessageType.Error));
        }

        public static void ValidateHitos(ICollection<Hito> hitos, Response response)
        {
            foreach (var hito in hitos)
            {
                if (hito.Details.Any(x => x.Quantity <= 0))
                    response.Messages.Add(new Message(Resources.Billing.Solfac.HitoQuantityRequired, MessageType.Error));

                if (hito.Details.Any(x => x.UnitPrice <= 0))
                    response.Messages.Add(new Message(Resources.Billing.Solfac.HitoUnitPriceRequired, MessageType.Error));
            }
        }

        public static void ValidatePercentage(Solfac solfac, Response response)
        {
            var totalPercentage = solfac.BuenosAiresPercentage + solfac.CapitalPercentage +
                                  solfac.OtherProvince1Percentage + solfac.OtherProvince2Percentage +
                                  solfac.OtherProvince3Percentage;

            if (totalPercentage != 100)
                response.Messages.Add(new Message(Resources.Billing.Solfac.TotalPercentageError, MessageType.Error));

            if (solfac.CapitalPercentage < 0 || solfac.BuenosAiresPercentage < 0 ||
                solfac.OtherProvince1Percentage < 0 || solfac.OtherProvince2Percentage < 0 ||
                solfac.OtherProvince3Percentage < 0)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.PercentageLessThan0, MessageType.Error));
            }
        }

        public static void ValidateTimeLimit(Solfac solfac, Response response)
        {
            if (solfac.PaymentTermId <= 0)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.TimeLimitLessThan0, MessageType.Error));
            }
        }

        public static void ValidateImputationNumber(Solfac solfac, Response response)
        {
            if (string.IsNullOrWhiteSpace(solfac.ImputationNumber1))
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.ImputationNumberEmpty, MessageType.Error));
            }
        }

        public static void ValidateContractNumber(Solfac solfac, Response response)
        {
            if (string.IsNullOrWhiteSpace(solfac.ContractNumber))
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.ContractNumberEmpty, MessageType.Error));
            }
        }

        public static void ValidateCreditNote(Solfac solfac, ISolfacRepository solfacRepository, Response response)
        {
            var solfacId = solfac.Hitos.First().SolfacId;

            var totalLimit = solfacRepository.GetTotalAmountById(solfacId);

            var hitosTotalImport = solfac.Hitos.Sum(s => s.Details.Sum(d => d.Total));

            if (hitosTotalImport > totalLimit)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CreditNoteTotalExceededError, MessageType.Error));
            }
        }

        public static void ValidateBusinessName(Solfac solfac, Response<Solfac> response)
        {
            if (string.IsNullOrWhiteSpace(solfac.BusinessName))
            {
                response.AddError(Resources.Billing.Solfac.BusinessNameRequired);
            }
        }

        public static void ValidateDetails(ICollection<Hito> solfacHitos, Response response)
        {
            var details = solfacHitos.Sum(x => x.Details.Count);

            if (details == 0)
            {
                response.AddError(Resources.Billing.Solfac.DetailsRequired);
            }
        }
    }
}
