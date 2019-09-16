using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Models.ManagementReport;

namespace Sofco.Core.DAL.ManagementReport
{
    public interface IManagementReportBillingRepository : IBaseRepository<ManagementReportBilling>
    {
        ManagementReportBilling GetById(int IdManamentReport);
        IList<ManagementReportBilling> GetByManagementReportAndDates(int managementReportId, DateTime startDate, DateTime endDate);
        ManagementReportBilling GetByManagementReportIdAndDate(int modelManagementReportId, DateTime monthYear);
        void Close(ManagementReportBilling billing);
        void AddResource(ResourceBilling domain);
        void DeleteResource(ResourceBilling resourceBilling);
        void UpdateResource(ResourceBilling domain);
        IList<ResourceBilling> GetResources(int idBilling, string hitoId);
        void DeleteResources(IList<ResourceBilling> reportBillingResourceBillings);
        int GetResourcesCount(int reportBillingId);
        IList<ResourceBilling> GetResources(int billingId);
    }
}
