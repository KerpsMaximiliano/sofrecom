using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class CostDetailRepository : BaseRepository<CostDetail>, ICostDetailRepository
    {
        public CostDetailRepository(SofcoContext context) : base(context)
        {
        }

        public List<CostDetailType> GetResourceTypes()
        {
            return context.CostDetailTypes.OrderBy(t => t.Id).ToList();
        }


    }
}
