using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Core.Models.Billing;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IProjectService
    {
        IList<CrmProjectHito> GetHitosByProject(string projectId);

        Response<IList<Project>> GetProjects(string serviceId);

        Response<IList<SelectListModel>> GetProjectsOptions(string serviceId);

        Response<Project> GetProjectById(string projectId);

        Response<IList<OpportunityOption>> GetOpportunities(string serviceId);

        IList<PurchaseOrderWidgetModel> GetPurchaseOrders(string projectId);

        Response Update();
    }
}