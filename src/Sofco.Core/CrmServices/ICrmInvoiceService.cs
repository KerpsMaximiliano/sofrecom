﻿using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Crm;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.CrmServices
{
    public interface ICrmInvoiceService
    {
        Result<List<CrmHito>> GetHitosToExpire(int daysToExpire);

        Result<string> CreateHitoBySolfac(Solfac solfac);

        Response UpdateHitos(ICollection<Hito> hitos);

        void UpdateHitoStatus(List<Hito> hitos, HitoStatus hitoStatus);

        void UpdateHitoInvoice(IList<Hito> list, SolfacStatusParams parameters);
    }
}
