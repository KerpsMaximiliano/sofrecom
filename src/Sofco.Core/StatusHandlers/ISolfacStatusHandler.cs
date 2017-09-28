﻿using Sofco.Core.Config;
using Sofco.Core.DAL.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
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
        HitoStatus GetHitoStatus();
        void SaveStatus(Solfac solfac, SolfacStatusParams parameters, ISolfacRepository _solfacRepository);
    }
}
