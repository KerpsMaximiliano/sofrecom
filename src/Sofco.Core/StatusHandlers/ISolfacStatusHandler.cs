using System.Collections.Generic;
using Sofco.Core.Config;
using Sofco.Core.DAL.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface ISolfacStatusHandler
    {
        Response Validate(Solfac solfac, SolfacStatusParams parameters);
        string GetBodyMail(Solfac solfac, string siteUrl);
        string GetSubjectMail(Solfac solfac);
        string GetRecipients(Solfac solfac, EmailConfig emailConfig);
        string GetSuccessMessage();
        void SaveStatus(Solfac solfac, SolfacStatusParams parameters, ISolfacRepository solfacRepository);
        void UpdateHitos(ICollection<string> getHitosIdsBySolfacId, Solfac solfac, string url);
    }
}
