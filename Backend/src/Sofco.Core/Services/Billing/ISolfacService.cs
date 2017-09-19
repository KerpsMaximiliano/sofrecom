﻿using System.Collections.Generic;
using Sofco.Core.Config;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISolfacService
    {
        Response<Solfac> Add(Solfac solfac);
        IList<Solfac> Search(SolfacParams parameter);
        IList<Hito> GetHitosByProject(string projectId);
        IList<Solfac> GetByProject(string projectId);
        Response<Solfac> GetById(int id);
        Response ChangeStatus(Solfac solfac, SolfacStatus status, EmailConfig emailConfig);
        Response ChangeStatus(int id, SolfacStatus status, EmailConfig emailConfig);
        Response Delete(int id);
    }
}
