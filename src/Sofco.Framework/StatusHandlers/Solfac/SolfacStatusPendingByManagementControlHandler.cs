using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusPendingByManagementControlHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        public SolfacStatusPendingByManagementControlHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private const string MailBody = "<font size='3'>" +
                                        "<span style='font-size:12pt'>" +
                                        "Estimados, </br></br>" +
                                        "Se ha cargado una solfac que requiere revisión y aprobación </br>" +
                                        "Para acceder, por favor ingresar al siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                        "Muchas gracias." +
                                        "</span>" +
                                        "</font>";

        private const string MailSubject = "SOLFAC - {0} - {1} - {2} - {3}";

        private const string MailBodyToUser = "<font size='3'>" +
                                              "<span style='font-size:12pt'>" +
                                              "Estimado, </br></br>" +
                                              "Se ha iniciado el proceso de facturación de la solicitud del asunto. Para acceder al misma, " +
                                              "por favor ingresar al siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                              "Muchas gracias." +
                                              "</span>" +
                                              "</font>";

        private const string MailSubjectToUser = "SOLFAC: INICIO PROCESO - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            SolfacValidationHelper.ValidateDetails(solfac.Hitos, response);

            if (solfac.Status == SolfacStatus.SendPending || solfac.Status == SolfacStatus.ManagementControlRejected || solfac.Status == SolfacStatus.RejectedByDaf)
            {
                if (solfac.InvoiceRequired && solfac.DocumentTypeId != SolfacDocumentType.CreditNoteA 
                                           && solfac.DocumentTypeId != SolfacDocumentType.CreditNoteB
                                           && solfac.DocumentTypeId != SolfacDocumentType.DebitNote
                                           && !unitOfWork.SolfacRepository.HasInvoices(solfac.Id))
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.SolfacHasNoInvoices, MessageType.Error));
                }

                if (!response.HasErrors() && (!unitOfWork.SolfacRepository.HasAttachments(solfac.Id) && !unitOfWork.SolfacCertificateRepository.HasCertificates(solfac.Id)))
                {
                    response.Messages.Add(new Message(Resources.Billing.Solfac.SolfacHasNoAttachments, MessageType.Warning));
                }

                return response;
            }

            response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            return response;
        }

        private string GetBodyMail(Model.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(MailBody, link);
        }

        private string GetSubjectMail(Model.Models.Billing.Solfac solfac)
        {
            return string.Format(MailSubject, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipients(Model.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            return unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode);
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.PendingByManagementControlSuccess;
        }

        private HitoStatus GetHitoStatus()
        {
            return HitoStatus.Pending;
        }

        public void SaveStatus(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Model.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            unitOfWork.SolfacRepository.UpdateStatus(solfacToModif);
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
                        var stringContent = new StringContent($"StatusCode={(int)GetHitoStatus()}", Encoding.UTF8, "application/x-www-form-urlencoded");
                        response = await client.PutAsync($"/api/InvoiceMilestone/{item}", stringContent);

                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public void SendMail(IMailSender mailSender, Model.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            var subjectToCdg = GetSubjectMail(solfac);
            var bodyToCdg = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipientsToCdg = GetRecipients(solfac, emailConfig);

            mailSender.Send(recipientsToCdg, subjectToCdg, bodyToCdg);

            var subject = string.Format(MailSubjectToUser, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
            var body = string.Format(MailBodyToUser, $"{emailConfig.SiteUrl}billing/solfac/{solfac.Id}");
            var recipients = solfac.UserApplicant.Email;

            mailSender.Send(recipients, subject, body);
        }
    }
}
