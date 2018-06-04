using System.Collections.Generic;
using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface ISolfacStatusHandler
    {
        Response Validate(Solfac solfac, SolfacStatusParams parameters);

        string GetSuccessMessage();

        void SaveStatus(Solfac solfac, SolfacStatusParams parameters);

        void UpdateHitos(ICollection<string> getHitosIdsBySolfacId, Solfac solfac, string url);

        void SendMail(IMailSender mailSender, Solfac solfac, EmailConfig emailConfig);
    }
}
