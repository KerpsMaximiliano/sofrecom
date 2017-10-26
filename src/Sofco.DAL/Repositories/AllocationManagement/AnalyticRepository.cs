﻿using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class AnalyticRepository : BaseRepository<Analytic>, IAnalyticRepository
    {
        public AnalyticRepository(SofcoContext context) : base(context)
        {
        }

        public bool Exist(int id)
        {
            return context.Analytics.Any(x => x.Id == id);
        }
    }
}
