﻿using Sofco.Core.DAL;
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
using Sofco.Domain.Models.ManagementReport;

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

                response.Data.Projects.Add(new ProjectOption { Id = project.CrmId, Text = project.Name, OpportunityId = project.OpportunityId, OpportunityNumber = project.OpportunityNumber });

                foreach (var hito in crmProjectHitos.OrderBy(x => x.StartDate))
                {
                    if (hito.Status.Equals("Cerrado")) continue;

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
                        {
                            rowItem.SolfacId = existHito.SolfacId;

                            if (existHito.Solfac.CurrencyExchange.HasValue && existHito.Solfac.CurrencyExchange > 0)
                            {
                                rowItem.ValuePesos = hito.Ammount * existHito.Solfac.CurrencyExchange.Value;
                            }
                        }

                        if (hito.Status.Equals("A ser facturado"))
                            rowItem.Status = HitoStatus.ToBeBilled.ToString();

                        billingRowItem.MonthValues.Add(rowItem);

                        FillTotalBilling(response, hito, dates);

                        response.Data.Rows.Add(billingRowItem);
                    }
                }

                foreach (var billingTotal in response.Data.Totals)
                {
                    billingTotal.MonthValues = billingTotal.MonthValues.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
                }
            }

            return response;
        }

        public Response<CostDetailModel> GetCostDetail(string serviceId)
        {
            var response = new Response<CostDetailModel> { Data = new CostDetailModel() };
            try
            {
                var analytic = unitOfWork.AnalyticRepository.GetByService(serviceId);

                if (analytic == null)
                {
                    response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                    return response;
                }

                response.Data.AnalyticId = analytic.Id;
                //Obtengo los meses que tiene la analitica
                response.Data.MonthsHeader = new List<MonthDetailCost>();

                response.Data.ManagerId = analytic.Manager.ExternalManagerId;

                for (DateTime date = new DateTime(analytic.StartDateContract.Year, analytic.StartDateContract.Month, 1).Date; date.Date <= analytic.EndDateContract.Date; date = date.AddMonths(1))
                {
                    var monthHeader = new MonthDetailCost();
                    monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                    monthHeader.MonthYear = date;

                    response.Data.MonthsHeader.Add(monthHeader);
                }

                //Obtengo los datos de las celdas
                var costDetails = unitOfWork.CostDetailRepository.GetByAnalytic(analytic.Id);
                var CostDetailEmployees = costDetails.Where(cd => cd.Type.Name == CostDetailTypeResource.Empleados.ToString()).ToList();
                var FundedResources = costDetails.Where(cd => cd.Type.Name != CostDetailTypeResource.Empleados.ToString()).ToList();

                //Obtengo los tipos de Recursos
                List<CostDetailResourceType> Types = unitOfWork.CostDetailRepository.GetResourceTypes();
                List<CostDetailResourceType> TypesFundedResources = Types.Where(t => t.Name != CostDetailTypeResource.Empleados.ToString()).ToList();
                CostDetailResourceType EmployeeType = Types.Where(t => t.Name == CostDetailTypeResource.Empleados.ToString()).FirstOrDefault();

                //Mapeo Los empleados      
                response.Data.CostEmployees = FillCostEmployeesByMonth(analytic.Id, response.Data.MonthsHeader, CostDetailEmployees, EmployeeType);
                //Mapeo Los demas datos
                var AllCostResources = FillFundedResoursesByMonth(analytic.Id, response.Data.MonthsHeader, FundedResources, TypesFundedResources);

                response.Data.FundedResources = AllCostResources.Where(r => r.show == true).ToList();
                response.Data.OtherResources = AllCostResources.Where(r => r.show == false).OrderBy(r => r.Display).ToList();

            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response UpdateCostDetail(CostDetailModel pDetailCost)
        {
            var response = new Response();
            try
            {
                var costDetails = unitOfWork.CostDetailRepository.GetByAnalytic(pDetailCost.AnalyticId);

                List<CostResource> resources = new List<CostResource>();
                resources.AddRange(pDetailCost.CostEmployees);
                resources.AddRange(pDetailCost.FundedResources);

                var currentUser = userData.GetCurrentUser();

                foreach (var resource in resources)
                {
                    foreach (var month in resource.MonthsCost)
                    {
                        CostDetail entity = new CostDetail();

                        if (month.CostDetailId > 0)
                        {
                            entity = costDetails.Where(c => c.Id == month.CostDetailId).FirstOrDefault();

                            if (month.Value != entity.Cost || month.Charges != entity.Charges)
                            {
                                entity.Cost = month.Value ?? 0;
                                entity.Adjustment = month.Adjustment;
                                entity.Charges = month.Charges;
                                entity.ModifiedAt = DateTime.UtcNow;
                                entity.ModifiedById = currentUser.Id;

                                unitOfWork.CostDetailRepository.Update(entity);
                            }
                        }
                        else
                        {
                            if (month.Value > 0 || month.Charges > 0)
                            {
                                entity.IdAnalytic = pDetailCost.AnalyticId;
                                entity.Cost = month.Value ?? 0;
                                entity.Adjustment = month.Adjustment;
                                entity.Charges = month.Charges;
                                entity.MonthYear = month.MonthYear;
                                entity.TypeId = resource.TypeId;
                                entity.EmployeeId = resource.EmployeeId;
                                entity.CreatedAt = DateTime.UtcNow;
                                entity.CreatedById = currentUser.Id;

                                unitOfWork.CostDetailRepository.Insert(entity);
                            }
                        }
                    }
                }

                unitOfWork.Save();

                response.AddSuccess(Resources.Common.SaveSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response UpdateCostDetailMonth(CostDetailMonthModel pMonthDetail)
        {
            var response = new Response();
            CostDetailModel _detailModel = new CostDetailModel();
            _detailModel.CostEmployees = new List<CostResource>();
            _detailModel.FundedResources = new List<CostResource>();

            try
            {
                //Mapping
                _detailModel.AnalyticId = pMonthDetail.AnalyticId;

                foreach (var employee in pMonthDetail.Employees)
                {
                    CostResource cost = new CostResource();
                    cost.MonthsCost = new List<MonthDetailCost>();
                    MonthDetailCost month = new MonthDetailCost();

                    cost.EmployeeId = employee.EmployeeId;
                    cost.TypeId = employee.TypeId;

                    month.Value = employee.Salary;
                    month.Charges = employee.Charges;
                    month.MonthYear = employee.MonthYear;
                    month.CostDetailId = employee.CostDetailId;

                    cost.MonthsCost.Add(month);
                    _detailModel.CostEmployees.Add(cost);
                }

                foreach (var otherRes in pMonthDetail.OtherResources)
                {
                    CostResource cost = new CostResource();
                    cost.MonthsCost = new List<MonthDetailCost>();
                    MonthDetailCost month = new MonthDetailCost();

                    cost.TypeId = otherRes.TypeId;
                    month.Value = otherRes.Salary;
                    month.MonthYear = otherRes.MonthYear;
                    month.CostDetailId = otherRes.CostDetailId;

                    cost.MonthsCost.Add(month);
                    _detailModel.CostEmployees.Add(cost);
                }

                this.UpdateCostDetail(_detailModel);

                response.AddSuccess(Resources.Common.SaveSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

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

        private List<CostResource> FillCostEmployeesByMonth(int IdAnalytic, IList<MonthDetailCost> Months, List<CostDetail> CostDetailEmployees, CostDetailResourceType EmployeeType)
        {
            List<CostResource> costEmployees = new List<CostResource>();

            //Obtengo los empleados de la analitica
            var EmployeesAnalytic = unitOfWork.EmployeeRepository.GetByAnalyticWithWorkTimes(IdAnalytic);

            foreach (var employee in EmployeesAnalytic)
            {
                var detailEmployee = new CostResource();
                detailEmployee.MonthsCost = new List<MonthDetailCost>();

                detailEmployee.EmployeeId = employee.Id;
                detailEmployee.Display = employee.Name + " - " + employee.Id;
                detailEmployee.TypeId = EmployeeType.Id;
                detailEmployee.TypeName = EmployeeType.Name;

                foreach (var mounth in Months)
                {
                    var monthDetail = new MonthDetailCost();

                    var monthValue = CostDetailEmployees.Find(e => e.EmployeeId == employee.Id && new DateTime(e.MonthYear.Year, e.MonthYear.Month, 1).Date == mounth.MonthYear.Date);
                    if (monthValue != null)
                    {
                        monthDetail.Value = monthValue.Cost;
                        monthDetail.OriginalValue = monthValue.Cost;
                        monthDetail.Charges = monthValue.Charges;
                        monthValue.Adjustment = monthValue.Adjustment;
                        monthDetail.CostDetailId = monthValue.Id;
                    }

                    monthDetail.Display = mounth.Display;
                    monthDetail.MonthYear = mounth.MonthYear;

                    //Verifico si este mes el recurso se encontro en la analitica
                    var startDate = new DateTime(mounth.MonthYear.Year, mounth.MonthYear.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var alocation = employee.Allocations.Where(x => x.AnalyticId == IdAnalytic && x.StartDate >= startDate.Date && x.StartDate <= endDate.Date && x.Percentage > 0).ToList();
                    if (alocation.Any())
                    {
                        monthDetail.HasAlocation = true;
                    }
                    else
                    {
                        monthDetail.HasAlocation = false;
                        //Ticket 9471 si se quito la asignacion borrar el valor
                        monthDetail.Value = null;
                        monthDetail.OriginalValue = null;
                        monthDetail.Adjustment = null;
                    }

                    detailEmployee.MonthsCost.Add(monthDetail);
                }

                costEmployees.Add(detailEmployee);
            }

            return costEmployees.OrderBy(e => e.Display).ToList();
        }

        private List<CostResource> FillFundedResoursesByMonth(int IdAnalytic, IList<MonthDetailCost> Months, List<CostDetail> FundedResources, List<CostDetailResourceType> Types)
        {
            List<CostResource> fundedResources = new List<CostResource>();

            foreach (var type in Types)
            {
                var detailResource = new CostResource();
                detailResource.MonthsCost = new List<MonthDetailCost>();
                bool hasValue = false;

                detailResource.Display = type.Name;
                detailResource.TypeId = type.Id;
                detailResource.TypeName = type.Name;

                foreach (var mounth in Months)
                {
                    var monthDetail = new MonthDetailCost();

                    var monthValue = FundedResources.Find(r => r.TypeId == type.Id && new DateTime(r.MonthYear.Year, r.MonthYear.Month, 1).Date == mounth.MonthYear.Date);
                    if (monthValue != null)
                    {
                        monthDetail.Value = monthValue.Cost;
                        monthDetail.CostDetailId = monthValue.Id;
                        monthDetail.Charges = monthValue.Charges;
                        if (monthDetail.Value > 0)
                        {
                            hasValue = true;
                        }
                    }

                    monthDetail.Display = mounth.Display;
                    monthDetail.MonthYear = mounth.MonthYear;
                    detailResource.MonthsCost.Add(monthDetail);
                }

                //Separo los campos por defectos de los ocultos
                //var typeWithoutValue = detailResource.MonthsCost.Where(m => m.Value == null || m.Value == 0).ToList();
                if (type.Default == false)
                {
                    detailResource.OtherResource = true;
                }

                if (type.Default == true || hasValue)
                {
                    detailResource.show = true;
                }


                fundedResources.Add(detailResource);
            }

            return fundedResources;
        }
    }
}
