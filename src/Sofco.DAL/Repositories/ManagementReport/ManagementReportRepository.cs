﻿using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using System.Linq;
using Sofco.Core.Models.ManagementReport;
using System;
using System.Collections.Generic;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class ManagementReportRepository : BaseRepository<Domain.Models.ManagementReport.ManagementReport>, IManagementReportRepository
    {
        public ManagementReportRepository(SofcoContext context) : base(context)
        {
        }

        public Domain.Models.ManagementReport.ManagementReport GetById(int IdManamentReport)
        {
            var data = context.ManagementReports
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.CostDetailResources)
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.CostDetailProfiles)
                .Include(mr => mr.CostDetails)
                    .ThenInclude(x => x.CostDetailOthers)
                .SingleOrDefault(mr => mr.Id == IdManamentReport);

            return data;
        }

        public Domain.Models.ManagementReport.ManagementReport GetWithAnalytic(int managementReportId)
        {
            var data = context.ManagementReports
                .Include(x => x.Analytic)
                .ThenInclude(x => x.Manager)
                .SingleOrDefault(mr => mr.Id == managementReportId);

            return data;
        }

        public void UpdateStatus(Domain.Models.ManagementReport.ManagementReport report)
        {
            context.Entry(report).Property("Status").IsModified = true;
        }

        public bool AllMonthsAreClosed(int managementReportId)
        {
            return context.ManagementReportBillings.Where(x => x.ManagementReportId == managementReportId).All(x => x.Closed);
        }

        public List<Domain.Models.ManagementReport.ManagementReport> GetByDate(DateTime date)
        {
            var data = context.ManagementReports
                .Include(x => x.Analytic)
                .ThenInclude(x => x.Manager)
                .Where(x => x.StartDate.Date <= date.Date && x.EndDate.Date >= date.Date)
                .ToList();

            return data;
        }
    }
}
