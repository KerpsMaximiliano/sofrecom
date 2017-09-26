﻿using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusInvoicePendingHandler : ISolfacStatusHandler
    {
        private readonly IGroupRepository _groupRepository;

        public SolfacStatusInvoicePendingHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        private const string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimados, </br></br>" +
                                                "Control de Gestión ha aprobado la SOLFAC del asunto, que requiere su facturación. </br>" +
                                                "Para acceder, por favor ingresar al siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                                "Muchas gracias." +
                                            "</span>" +
                                        "</font>";

        private const string MailSubject = "SOLFAC - APROBADA por Control de Gestión - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.PendingByManagementControl)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.CannotChangeStatus, MessageType.Error));
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
            var group = _groupRepository.GetSingle(x => x.Id == emailConfig.CdgMail);
            return group.Email;
        }

        public string GetSuccessMessage()
        {
            return Resources.es.Billing.Solfac.InvoicePendingSuccess;
        }

        public HitoStatus GetHitoStatus()
        {
            return HitoStatus.ToBeBilled;
        }
    }
}
