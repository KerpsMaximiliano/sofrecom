﻿using System.Collections.Generic;
using Sofco.Model.Models.Billing;

namespace Sofco.Core.DAL.Billing
{
    public interface ISolfacDelegateRepository
    {
        List<SolfacDelegate> GetAll();

        SolfacDelegate Save(SolfacDelegate solfacDelegate);
    }
}