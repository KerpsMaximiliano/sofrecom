using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Sofco.Core.Config;
using Sofco.Core.DAL.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHandlers.Billing;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusAmountCashedHandler : ISolfacStatusHandler
    {
        private const string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimados, </br></br>" +
                                                "La SOLFAC del asunto se encuentra COBRADA. </br>" +
                                                "Para acceder, por favor ingresar al siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                                "Muchas gracias." +
                                            "</span>" +
                                        "</font>";

        private const string MailSubject = "SOLFAC - COBRADA - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.Invoiced)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }

            SolfacValidationHelper.ValidateCasheDate(parameters, response);

            return response;
        }

        public string GetBodyMail(Model.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(MailBody, link);
        }

        public string GetSubjectMail(Model.Models.Billing.Solfac solfac)
        {
            return string.Format(MailSubject, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        public string GetRecipients(Model.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            return solfac.UserApplicant.Email;
        }

        public string GetSuccessMessage()
        {
            return Resources.es.Billing.Solfac.InvoicedSuccess;
        }

        private HitoStatus GetHitoStatus()
        {
            return HitoStatus.Cashed;
        }

        public void SaveStatus(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters, ISolfacRepository solfacRepository)
        {
            var solfacToModif = new Model.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status, CashedDate = parameters.CashedDate };
            solfacRepository.UpdateStatusAndCashed(solfacToModif);
            solfac.CashedDate = parameters.CashedDate;
        }

        public async void UpdateHitos(ICollection<string> hitos, Model.Models.Billing.Solfac solfac, string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                HttpResponseMessage response;

                foreach (var item in hitos)
                {
                    try
                    {
                        var stringContent = new StringContent($"StatusCode={(int)GetHitoStatus()}&BillingDate={solfac.CashedDate.GetValueOrDefault().ToString("O")}", Encoding.UTF8, "application/x-www-form-urlencoded");
                        response = await client.PutAsync($"/api/InvoiceMilestone/{item}", stringContent);

                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}
