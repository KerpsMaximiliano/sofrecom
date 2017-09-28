﻿using System;
using Sofco.Core.Config;
using Sofco.Core.DAL.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusManagementControlRejectedHandler : ISolfacStatusHandler
    {
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
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }

            if (string.IsNullOrWhiteSpace(parameters.Comment))
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.CommentRequired, MessageType.Error));
            }

            if (!response.HasErrors())
            {
                MailBody = MailBody.Replace("*", parameters.Comment);
            }
            
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
            return Resources.es.Billing.Solfac.ManagementControlRejectedSuccess;
        }

        public HitoStatus GetHitoStatus()
        {
            return HitoStatus.Pending;
        }

        public void SaveStatus(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters, ISolfacRepository _solfacRepository)
        {
            var solfacToModif = new Model.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            _solfacRepository.UpdateStatus(solfacToModif);
        }
    }
}
