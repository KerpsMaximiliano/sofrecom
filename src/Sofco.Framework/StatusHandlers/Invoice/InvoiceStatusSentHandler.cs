using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusSentHandler : IInvoiceStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        public InvoiceStatusSentHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private string mailBody = "<font size='3'>" +
                                        "<span style='font-size:12pt'>" +
                                            "Estimados, </br></br>" +
                                            "Se ha cargado un REMITO que requiere revisión y generación (pdf). </br>" +
                                            "*" +
                                            "Para imprimirlo, utilice el documento anexado al registro. </br>" +
                                            "Una vez generado el pdf, por favor importarlo en el siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                            "Muchas gracias." +
                                        "</span>" +
                                    "</font>";

        private const string MailSubject = "REMITO - {0} - {1} - {2} - {3}";

        private const string MailBodyToUser = "<font size='3'>" +
                                              "<span style='font-size:12pt'>" +
                                              "Estimado, </br></br>" +
                                              "Se ha iniciado el proceso de generación del remito del asunto. Para acceder al mismo, " +
                                              "por favor ingresar al siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                              "Muchas gracias." +
                                              "</span>" +
                                          "</font>";

        private const string MailSubjectToUser = "REMITO: INICIO PROCESO - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var response = new Response();

            if (invoice.InvoiceStatus == InvoiceStatus.Approved || invoice.InvoiceStatus == InvoiceStatus.Cancelled ||
                invoice.InvoiceStatus == InvoiceStatus.Related)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.CannotSendToDaf, MessageType.Error));
            }

            if ((invoice.InvoiceStatus == InvoiceStatus.SendPending ||
                 invoice.InvoiceStatus == InvoiceStatus.Rejected) &&
                string.IsNullOrWhiteSpace(invoice.ExcelFileName))
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NeedExcelToSend, MessageType.Error));
            }

            if (!response.HasErrors())
            {
                if (string.IsNullOrWhiteSpace(parameters.Comment))
                    mailBody = mailBody.Replace("*", string.Empty);
                else
                    mailBody = mailBody.Replace("*", $"Comentarios: {parameters.Comment}. </br>");
            }

            return response;
        }

        private string GetBodyMail(Model.Models.Billing.Invoice invoice, string siteUrl)
        {
            var link = $"{siteUrl}billing/invoice/{invoice.Id}/project/{invoice.ProjectId}";

            return string.Format(mailBody, link);
        }

        private string GetSubjectMail(Model.Models.Billing.Invoice invoice)
        {
            return string.Format(MailSubject, invoice.AccountName, invoice.Service, invoice.Project, invoice.CreatedDate.ToString("yyyyMMdd"));
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Invoice.SentToDaf;
        }

        public string GetRecipients(Model.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            return unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);
        }

        public void SaveStatus(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Model.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Sent };
            unitOfWork.InvoiceRepository.UpdateStatus(invoiceToModif);
        }

        public void SendMail(IMailSender mailSender, Model.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            var subjectToDaf = GetSubjectMail(invoice);
            var bodyToDaf = GetBodyMail(invoice, emailConfig.SiteUrl);
            var recipientsToDaf = GetRecipients(invoice, emailConfig);

            mailSender.Send(recipientsToDaf, subjectToDaf, bodyToDaf);

            var subject = string.Format(MailSubjectToUser, invoice.AccountName, invoice.Service, invoice.Project, invoice.CreatedDate.ToString("yyyyMMdd"));
            var body = string.Format(MailBodyToUser, $"{emailConfig.SiteUrl}billing/invoice/{invoice.Id}/project/{invoice.ProjectId}");
            var recipients = invoice.User.Email;

            mailSender.Send(recipients, subject, body);
        }
    }
}
