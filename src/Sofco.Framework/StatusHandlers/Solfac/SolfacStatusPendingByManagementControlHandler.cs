using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusPendingByManagementControlHandler : ISolfacStatusHandler
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ISolfacRepository solfacRepository;

        public SolfacStatusPendingByManagementControlHandler(IGroupRepository groupRepository, ISolfacRepository solfacRepo)
        {
            _groupRepository = groupRepository;
            solfacRepository = solfacRepo;
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

        public Response Validate(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status == SolfacStatus.SendPending || solfac.Status == SolfacStatus.ManagementControlRejected)
            {
                if (!solfacRepository.HasAttachments(solfac.Id))
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacHasNoAttachments, MessageType.Warning));
                }

                return response;
            }

            response.Messages.Add(new Message(Resources.es.Billing.Solfac.CannotChangeStatus, MessageType.Error));
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
            return _groupRepository.GetEmail(emailConfig.CdgCode);
        }

        public string GetSuccessMessage()
        {
            return Resources.es.Billing.Solfac.PendingByManagementControlSuccess;
        }

        private HitoStatus GetHitoStatus()
        {
            return HitoStatus.Pending;
        }

        public void SaveStatus(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters, ISolfacRepository solfacRepository)
        {
            var solfacToModif = new Model.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            solfacRepository.UpdateStatus(solfacToModif);
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
    }
}
