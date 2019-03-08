using System.Collections.Generic;
using Sofco.Core.Config;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface ISolfacStatusHandler
    {
        Response Validate(Solfac solfac, SolfacStatusParams parameters);

        string GetSuccessMessage();

        void SaveStatus(Solfac solfac, SolfacStatusParams parameters);

        void UpdateHitos(ICollection<string> getHitosIdsBySolfacId, Solfac solfac);

        void SendMail(Solfac solfac, EmailConfig emailConfig);
    }
}
