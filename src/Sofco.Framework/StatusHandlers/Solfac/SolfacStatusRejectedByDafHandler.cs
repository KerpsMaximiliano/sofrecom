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
    public class SolfacStatusManagementControlRejectedHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        public SolfacStatusManagementControlRejectedHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimados, </br></br>" +
                                                "La SOLFAC del asunto ha sido RECHAZADA por Control de Gestión, por el siguiente motivo: </br>" +
                                                "*" +
                                                "</br>" +
                                                "Por favor, ingresar al siguiente <a href='{0}' target='_blank'>link</a> para modificar el formulario " +
                                                "y enviar nuevamente </br></br>" +
                                                "Muchas gracias." +
                                            "</span>" +
                                        "</font>";

        private const string MailSubject = "SOLFAC - RECHAZADA por Control de Gestión - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.PendingByManagementControl)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }

            SolfacValidationHelper.ValidateComments(parameters, response);

            if (!response.HasErrors())
            {
                MailBody = MailBody.Replace("*", parameters.Comment);
            }
            
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

        private string GetRecipients(Model.Models.Billing.Solfac solfac, string mailCdg)
        {
            return $"{solfac.UserApplicant.Email};{mailCdg}";
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.ManagementControlRejectedSuccess;
        }

        public HitoStatus GetHitoStatus()
        {
            return HitoStatus.Pending;
        }

        public void SaveStatus(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Model.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            unitOfWork.SolfacRepository.UpdateStatus(solfacToModif);

            if (solfac.PurchaseOrder == null) return;

            solfac.PurchaseOrder.Balance = solfac.PurchaseOrder.Balance + solfac.TotalAmount;

            unitOfWork.PurchaseOrderRepository.UpdateBalance(solfac.PurchaseOrder);
        }

        public async void UpdateHitos(ICollection<string> hitos, Model.Models.Billing.Solfac solfac, string url)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                foreach (var item in hitos)
                {
                    try
                    {
                        var purchaseOrder = solfac.PurchaseOrder.Number;

                        var stringContent = new StringContent(
                            $"StatusCode={(int)GetHitoStatus()}&PurchaseOrder={purchaseOrder}",
                            Encoding.UTF8, "application/x-www-form-urlencoded");

                        var response = await client.PutAsync($"/api/InvoiceMilestone/{item}", stringContent);

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
            var mailCdg = unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode);

            var subject = GetSubjectMail(solfac);
            var body = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipients = GetRecipients(solfac, mailCdg);

            mailSender.Send(recipients, subject, body);
        }
    }
}
