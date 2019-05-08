﻿using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using System.Linq;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class ManagementReportRepository : BaseRepository<Domain.Models.ManagementReport.ManagementReport>, IManagementReportRepository
    {
        public ManagementReportRepository(SofcoContext context) : base(context)
        {
        }

        public Sofco.Domain.Models.ManagementReport.ManagementReport GetById(int IdManamentReport)
        {
            var data = context.ManagementReports
                .Where(mr => mr.Id == IdManamentReport)
                .Include(mr => mr.CostDetails)
                    //.ThenInclude(cd => cd.cos
                    //.Include(mr => mr.CostDetails.Select(prof => prof.CostDetailProfiles))
                    //.Include(mr => mr.CostDetails.Select(other => other.CostDetailOthers))
                .FirstOrDefault();

            return data;
        }
    }
}
