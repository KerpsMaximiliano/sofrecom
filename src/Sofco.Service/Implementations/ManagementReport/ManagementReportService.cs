using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Service.Implementations.ManagementReport
{
    public class ManagementReportService : IManagementReportService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ManagementReportService> logger;
        private readonly IRoleManager roleManager;
        private readonly IUserData userData;
        private readonly IProjectData projectData;

        public ManagementReportService(IUnitOfWork unitOfWork,
            ILogMailer<ManagementReportService> logger,
            IUserData userData,
            IProjectData projectData,
            IRoleManager roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            this.projectData = projectData;
            this.roleManager = roleManager;
        }

        public Response<ManagementReportDetail> GetDetail(string serviceId)
        {
            var response = new Response<ManagementReportDetail> { Data = new ManagementReportDetail() };

            var analytic = unitOfWork.AnalyticRepository.GetByService(serviceId);

            if (analytic != null)
            {
                if (!CheckRoles(analytic))
                {
                    response.AddError(Resources.Common.Forbidden);
                    return response;
                }

                response.Data.StartDate = analytic.StartDateContract;
                response.Data.EndDate = analytic.EndDateContract;
            }
            else
            {
                response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                return response;
            }

            var service = unitOfWork.ServiceRepository.GetByIdCrm(serviceId);

            if (service != null)
            {
                response.Data.Analytic = service.Analytic;
                response.Data.ServiceType = service.ServiceType;
                response.Data.SolutionType = service.SolutionType;
                response.Data.TechnologyType = service.TechnologyType;
                response.Data.Manager = service.Manager;
                response.Data.Name = service.Name;
                response.Data.AccountName = service.AccountName;
            }

            var projects = unitOfWork.ProjectRepository.GetAllActives(serviceId);

            if (projects != null && projects.Any())
            {
                response.Data.Opportunities = projects.Select(x => $"{x.OpportunityNumber} - {x.OpportunityName}").ToList();
            }

            var purchaseOrders = unitOfWork.PurchaseOrderRepository.GetByService(serviceId);

            if (purchaseOrders != null && purchaseOrders.Any())
            {
                var diccionary = new Dictionary<string, decimal>();

                response.Data.PurchaseOrders = purchaseOrders.Select(x => x.Title).ToList();
                response.Data.Ammounts = new List<AmmountItem>();

                foreach (var purchaseOrder in purchaseOrders)
                {
                    foreach (var detail in purchaseOrder.AmmountDetails)
                    {
                        if (diccionary.ContainsKey(detail.Currency.Text))
                        {
                            diccionary[detail.Currency.Text] += detail.Ammount;
                        }
                        else
                        {
                            diccionary.Add(detail.Currency.Text, detail.Ammount);
                        }
                    }
                }

                foreach (var key in diccionary.Keys)
                {
                    response.Data.Ammounts.Add(new AmmountItem { Currency = key, Value = diccionary[key] });
                }
            }

            return response;
        }

        public Response<BillingDetail> GetBilling(string serviceId)
        {
            var response = new Response<BillingDetail> { Data = new BillingDetail() };

            var analytic = unitOfWork.AnalyticRepository.GetByService(serviceId);

            if (analytic == null)
            {
                response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                return response;
            }

            var projects = unitOfWork.ProjectRepository.GetAllActives(serviceId);

            var today = DateTime.UtcNow;

            var dates = SetDates(analytic, today);

            response.Data.MonthsHeader = new List<MonthBillingHeaderItem>();
            response.Data.Projects = new List<ProjectOption>();

            response.Data.ManagerId = analytic.Manager.ExternalManagerId;

            for (DateTime date = dates.Item1.Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
            {
                var monthHeader = new MonthBillingHeaderItem();
                monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                monthHeader.Year = date.Year;
                monthHeader.Month = date.Month;
                response.Data.MonthsHeader.Add(monthHeader);
            }

            response.Data.Rows = new List<BillingRowItem>();

            foreach (var project in projects)
            {
                var crmProjectHitos = projectData.GetHitos(project.CrmId);

                response.Data.Projects.Add(new ProjectOption { Id = project.CrmId, Text = project.Name, OpportunityId = project.OpportunityId });

                foreach (var hito in crmProjectHitos)
                {
                    if (hito.StartDate.Date >= dates.Item1.Date && hito.StartDate.Date <= dates.Item2.Date)
                    {
                        var billingRowItem = new BillingRowItem
                        {
                            Description = hito.Name,
                            MonthValues = new List<MonthBiilingRowItem>
                            {
                                new MonthBiilingRowItem
                                {
                                    Month = hito.StartDate.Month,
                                    Year = hito.StartDate.Year,
                                    Value = hito.Ammount
                                }
                            }
                        };

                        response.Data.Rows.Add(billingRowItem);
                    }
                }
            }

            return response;
        }

        private Tuple<DateTime, DateTime> SetDates(Analytic analytic, DateTime today)
        {
            DateTime startDate;
            DateTime endDate;

            if (analytic.StartDateContract.Year <= today.Year)
            {
                startDate = analytic.StartDateContract.Date;

                endDate = analytic.EndDateContract.Year > today.Year
                    ? new DateTime(today.Year, 12, 31)
                    : analytic.EndDateContract.Date;
            }
            else
            {
                startDate = new DateTime(today.Year, 1, 1);

                endDate = analytic.EndDateContract.Year > today.Year
                    ? new DateTime(today.Year, 12, 31)
                    : analytic.EndDateContract.Date;
            }

            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        private bool CheckRoles(Analytic analytic)
        {
            var currentUser = userData.GetCurrentUser();

            if (roleManager.IsDirector() || roleManager.IsCdg())
            {
                return true;
            }
            else if (roleManager.IsManager() && currentUser.Id == analytic.ManagerId.GetValueOrDefault())
            {
                return true;
            }

            return false;
        }
    }
}
