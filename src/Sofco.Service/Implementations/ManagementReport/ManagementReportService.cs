using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Enums;

namespace Sofco.Service.Implementations.ManagementReport
{
    public class ManagementReportService : IManagementReportService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ISolfacService solfacService;
        private readonly ILogMailer<ManagementReportService> logger;
        private readonly IRoleManager roleManager;
        private readonly IUserData userData;
        private readonly IProjectData projectData;

        public ManagementReportService(IUnitOfWork unitOfWork,
            ILogMailer<ManagementReportService> logger,
            IUserData userData,
            IProjectData projectData,
            ISolfacService solfacService,
            IRoleManager roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            this.projectData = projectData;
            this.roleManager = roleManager;
            this.solfacService = solfacService;
        }

        public Response<ManagementReportDetail> GetDetail(string serviceId)
        {
            var response = new Response<ManagementReportDetail> { Data = new ManagementReportDetail() };

            var analytic = unitOfWork.AnalyticRepository.GetByServiceForManagementReport(serviceId);

            if (analytic != null)
            {
                if (!CheckRoles(analytic))
                {
                    response.AddError(Resources.Common.Forbidden);
                    return response;
                }

                response.Data.StartDate = analytic.StartDateContract;
                response.Data.EndDate = analytic.EndDateContract;
                response.Data.ServiceType = analytic.ServiceType?.Text;
                response.Data.SolutionType = analytic.Solution?.Text;
                response.Data.TechnologyType = analytic.Technology?.Text;
                response.Data.Manager = analytic.Manager?.Name;
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
                monthHeader.ResourceQuantity = unitOfWork.AllocationRepository.GetResourceQuantityByDate(analytic.Id, new DateTime(date.Year, date.Month, 1));

                response.Data.MonthsHeader.Add(monthHeader);
            }

            response.Data.Rows = new List<BillingHitoItem>();
            response.Data.Totals = new List<BillingTotal>();

            foreach (var project in projects)
            {
                var crmProjectHitos = projectData.GetHitos(project.CrmId);

                var hitos = solfacService.GetHitosByProject(project.CrmId);

                response.Data.Projects.Add(new ProjectOption { Id = project.CrmId, Text = project.Name, OpportunityId = project.OpportunityId });

                foreach (var hito in crmProjectHitos.OrderBy(x => x.StartDate))
                {
                    if (hito.StartDate.Date >= dates.Item1.Date && hito.StartDate.Date <= dates.Item2.Date)
                    {
                        var existHito = hitos.SingleOrDefault(x => x.ExternalHitoId == hito.Id);
                        
                        var billingRowItem = new BillingHitoItem
                        {
                            Description = $"{project.OpportunityNumber} - {hito.Name} - ({hito.Money})",
                            Id = hito.Id,
                            ProjectId = project.CrmId,
                            ProjectName = $"{project.OpportunityNumber} {project.Name}",
                            CurrencyId = hito.MoneyId,
                            Date = hito.StartDate,
                            MonthValues = new List<MonthBiilingRowItem>()
                        };

                        var rowItem = new MonthBiilingRowItem
                        {
                            Month = hito.StartDate.Month,
                            Year = hito.StartDate.Year,
                            Value = hito.Ammount,
                            Status = hito.Status
                        };

                        if (existHito != null)
                            rowItem.SolfacId = existHito.SolfacId;

                        if (hito.Status.Equals("A ser facturado"))
                            rowItem.Status = HitoStatus.ToBeBilled.ToString();

                        billingRowItem.MonthValues.Add(rowItem);

                        FillTotalBilling(response, hito, dates);

                        response.Data.Rows.Add(billingRowItem);
                    }
                }

                foreach (var billingTotal in response.Data.Totals)
                {
                    billingTotal.MonthValues = billingTotal.MonthValues.OrderBy(x => x.Month).ThenBy(x => x.Year).ToList();
                }
            }

            return response;
        }

        public Response<CostDetail> GetCostDetail(string serviceId)
        {
            var response = new Response<CostDetail> { Data = new CostDetail() };

            var analytic = unitOfWork.AnalyticRepository.GetByService(serviceId);

            if (analytic == null)
            {
                response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                return response;
            }

            //Obtengo los meses que tiene la analitica
            response.Data.MonthsHeader = new List<MonthDetailCost>();

            response.Data.ManagerId = analytic.Manager.ExternalManagerId;

            for (DateTime date = analytic.StartDateContract.Date; date.Date <= analytic.EndDateContract.Date; date = date.AddMonths(1))
            {
                var monthHeader = new MonthDetailCost();
                monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                monthHeader.Year = date.Year;
                monthHeader.Month = date.Month;

                response.Data.MonthsHeader.Add(monthHeader);
            }

            //Obtengo los empleados de la analitica
            response.Data.Employees = unitOfWork.EmployeeRepository.GetByAnalyticWithWorkTimes(analytic.Id);

            return response;
        }

        private void FillTotalBilling(Response<BillingDetail> response, CrmProjectHito hito, Tuple<DateTime, DateTime> dates)
        {
            var totalBilling = response.Data.Totals.FirstOrDefault(x => x.CurrencyId == hito.MoneyId);

            if (totalBilling != null)
            {
                var month = totalBilling.MonthValues.FirstOrDefault(x => x.Month == hito.StartDate.Month && x.Year == hito.StartDate.Year);

                if (month != null)
                {
                    month.Value += hito.Ammount;
                }
            }
            else
            {
                totalBilling = new BillingTotal
                {
                    CurrencyId = hito.MoneyId,
                    CurrencyName = hito.Money,
                    MonthValues = new List<MonthBiilingRowItem>()
                };

                for (DateTime date = dates.Item1.Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
                {
                    MonthBiilingRowItem biilingRowItem;

                    if (hito.StartDate.Month == date.Month && hito.StartDate.Year == date.Year)
                    {
                        biilingRowItem = new MonthBiilingRowItem { Year = date.Year, Month = date.Month, Value = hito.Ammount };
                    }
                    else
                    {
                        biilingRowItem = new MonthBiilingRowItem { Year = date.Year, Month = date.Month, Value = 0 };
                    }

                    totalBilling.MonthValues.Add(biilingRowItem);
                }

                response.Data.Totals.Add(totalBilling);
            }
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
