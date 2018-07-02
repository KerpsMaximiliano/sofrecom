using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Core.Models.Billing;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IProjectService
    {
        IList<CrmProjectHito> GetHitosByProject(string projectId);

        Response<IList<CrmProject>> GetProjects(string serviceId);

        Response<IList<SelectListModel>> GetProjectsOptions(string serviceId);

        Response<CrmProject> GetProjectById(string projectId);

        Response<IList<OpportunityOption>> GetOpportunities(string serviceId);

        IList<PurchaseOrderWidgetModel> GetPurchaseOrders(string projectId);
    }
}