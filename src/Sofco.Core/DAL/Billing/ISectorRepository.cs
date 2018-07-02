﻿using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Model.Utils;

namespace Sofco.Core.DAL.Billing
{
    public interface ISectorRepository : IBaseRepository<Sector>
    {
        new List<Sector> GetAll();
    }
}