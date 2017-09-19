using Sofco.Core.Config;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface ISolfacStatusHandler
    {
        Response Validate(Solfac solfac);
        string GetBodyMail(Solfac solfac, string siteUrl);
        string GetSubjectMail(Solfac solfac);
        string GetRecipients(Solfac solfac, EmailConfig emailConfig);
        string GetSuccessMessage();
    }
}
