using Sofco.Core.DAL.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using System;
using System.Collections.Generic;

namespace Sofco.Framework.ValidationHandlers.Billing
{
    public static class SolfacValidationHelper
    {
        public static void ValidateCasheDate(SolfacStatusParams parameters, Response response)
        {
            if (!parameters.CashedDate.HasValue)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.CashedDateRequired, MessageType.Error));
            }
            else
            {
                if (parameters.CashedDate.Value.Date > DateTime.Today.Date)
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.CashedDateGreaterThanToday, MessageType.Error));
                }
            }
        }

        public static Solfac ValidateIfExistAndGetWithUser(int id, ISolfacRepository solfacRepository, Response response)
        {
            var solfac = solfacRepository.GetByIdWithUser(id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
            }

            return solfac;
        }

        public static Solfac ValidateIfExist(int id, ISolfacRepository _solfacRepository, Response response)
        {
            var solfac = _solfacRepository.GetSingle(x => x.Id == id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.NotFound, MessageType.Error));
            }

            return solfac;
        }

        public static void ValidateInvoiceDate(SolfacStatusParams parameters, Response response)
        {
            if (!parameters.InvoiceDate.HasValue)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.InvoiceDateRequired, MessageType.Error));
            }
            else
            {
                if (parameters.InvoiceDate.Value.Date > DateTime.Today.Date)
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.InvoiceDateGreaterThanToday, MessageType.Error));
                }
            }
        }

        public static void ValidateComments(SolfacStatusParams parameters, Response response)
        {
            if (string.IsNullOrWhiteSpace(parameters.Comment))
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.CommentRequired, MessageType.Error));
            }
        }

        public static void ValidateInvoiceCode(SolfacStatusParams parameters, ISolfacRepository solfacRepository, Response response)
        {
            if (string.IsNullOrWhiteSpace(parameters.InvoiceCode))
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.InvoiceCodeRequired, MessageType.Error));
            }
            else
            {
                if (solfacRepository.InvoiceCodeExist(parameters.InvoiceCode))
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.InvoiceCodeAlreadyExist, MessageType.Error));
                }
            }
        }

        public static void ValidateProvincePercentage(Solfac solfac, Response response)
        {
            if (solfac.OtherProvince1Percentage > 0 && solfac.Province1Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));

            if (solfac.OtherProvince2Percentage > 0 && solfac.Province2Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));

            if (solfac.OtherProvince3Percentage > 0 && solfac.Province3Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));
        }

        public static void ValidateHitos(IList<Hito> hitos, Response response)
        {
            foreach (var hito in hitos)
            {
                if (hito.Quantity <= 0)
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.HitoQuantityRequired, MessageType.Error));

                if (hito.UnitPrice <= 0)
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.HitoUnitPriceRequired, MessageType.Error));
            }
        }

        public static void ValidatePercentage(Solfac solfac, Response response)
        {
            var totalPercentage = solfac.BuenosAiresPercentage + solfac.CapitalPercentage +
                                  solfac.OtherProvince1Percentage + solfac.OtherProvince2Percentage +
                                  solfac.OtherProvince3Percentage;

            if (totalPercentage != 100)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.TotalPercentageError, MessageType.Error));

            if (solfac.CapitalPercentage < 0 || solfac.BuenosAiresPercentage < 0 ||
                solfac.OtherProvince1Percentage < 0 || solfac.OtherProvince2Percentage < 0 ||
                solfac.OtherProvince3Percentage < 0)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.PercentageLessThan0, MessageType.Error));
            }
        }

        public static void ValidateTimeLimit(Solfac solfac, Response response)
        {
            if (solfac.TimeLimit <= 0)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.TimeLimitLessThan0, MessageType.Error));
            }
        }
    }
}
