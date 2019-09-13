using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.ManagementReport;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.ManagementReport;
using System.Linq;

namespace Sofco.DAL.Repositories.ManagementReport
{
    public class ManagementReportBillingRepository : BaseRepository<ManagementReportBilling>, IManagementReportBillingRepository
    {
        public ManagementReportBillingRepository(SofcoContext context) : base(context)
        {
        }

        public ManagementReportBilling GetById(int IdManamentReport)
        {
            var data = context.ManagementReportBillings
                .Where(mr => mr.ManagementReportId == IdManamentReport)
                .FirstOrDefault();

            return data;
        }

        public IList<ManagementReportBilling> GetByManagementReportAndDates(int managementReportId, DateTime startDate, DateTime endDate)
        {
            return context.ManagementReportBillings.Where(x =>
                x.ManagementReportId == managementReportId && x.MonthYear.Date >= startDate.Date &&
                x.MonthYear.Date <= endDate.Date).ToList();
        }

        public ManagementReportBilling GetByManagementReportIdAndDate(int managementReportId, DateTime monthYear)
        {
            return context.ManagementReportBillings.SingleOrDefault(x =>
                x.ManagementReportId == managementReportId && x.MonthYear.Date == monthYear.Date);
        }

        public void Close(ManagementReportBilling billing)
        {
            context.Entry(billing).Property("Closed").IsModified = true;
        }

        public void AddResource(ResourceBilling domain)
        {
            context.ResourceBillings.Add(domain);
        }

        public void DeleteResource(ResourceBilling domain)
        {
            context.ResourceBillings.Remove(domain);
        }

        public void UpdateResource(ResourceBilling domain)
        {
            context.ResourceBillings.Update(domain);
        }

        public IList<ResourceBilling> GetResources(int idBilling, string hitoId)
        {
            return context.ResourceBillings.Where(x => x.ManagementReportBillingId == idBilling && x.HitoCrmId.Equals(hitoId)).ToList();
        }

        public void DeleteResources(IList<ResourceBilling> reportBillingResourceBillings)
        {
            context.ResourceBillings.RemoveRange(reportBillingResourceBillings);
        }

        public int GetResourcesCount(int reportBillingId)
        {
            return context.ResourceBillings.Count(x => x.ManagementReportBillingId == reportBillingId);
        }

        public IList<ResourceBilling> GetResources(int billingId)
        {
            return context.ResourceBillings.Where(x => x.ManagementReportBillingId == billingId).ToList();
        }
    }
}
