﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.Common;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using Sofco.Common.Settings;
using Microsoft.Extensions.Options;

namespace Sofco.Service.Implementations.ManagementReport
{
    public class ManagementReportStaffService : IManagementReportStaffService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ManagementReportStaffService> logger;
        private readonly IRoleManager roleManager;
        private readonly IUserData userData;
        private readonly IManagementReportService managementReportService;
        private readonly AppSetting appSetting;

        public ManagementReportStaffService(IUnitOfWork unitOfWork,
            ILogMailer<ManagementReportStaffService> logger,
            IUserData userData,
            IRoleManager roleManager,
            IManagementReportService managementReportService,
            IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            this.roleManager = roleManager;
            this.managementReportService = managementReportService;
            this.appSetting = appSettingOptions.Value;
        }

        public Response<ManagementReportStaffDetail> GetDetail(int id)
        {
            var response = new Response<ManagementReportStaffDetail> { Data = new ManagementReportStaffDetail() };

            var managementReport = unitOfWork.ManagementReportRepository.GetStaffById(id);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            if (!CheckRoles(managementReport.Analytic, response))
            {
                response.AddError(Resources.Common.Forbidden);
                return response;
            }

            var dates = SetDates(managementReport.Analytic);

            try
            {
                response.Data.StartDate = managementReport.Analytic.StartDateContract;
                response.Data.EndDate = managementReport.Analytic.EndDateContract;
                response.Data.Manager = managementReport.Analytic.Manager.Name;
                response.Data.ManagerId = managementReport.Analytic.ManagerId;
                response.Data.Analytic = $"{managementReport.Analytic.Title} - {managementReport.Analytic.Name}";
                response.Data.AnalyticId = managementReport.AnalyticId;
                response.Data.ManagementReportId = managementReport.Id;
                response.Data.ManamementReportStartDate = managementReport.StartDate;
                response.Data.ManamementReportEndDate = managementReport.EndDate;
                response.Data.Sector = managementReport.Analytic.Sector.Text;
                response.Data.Status = managementReport.Status;
                response.Data.StateGenerated = managementReport.StateGenerated;

                response.Data.Months = new List<CurrencyExchangeModel>();

                var currencyExchanges = unitOfWork.CurrencyExchangeRepository.Get(dates.Item1.Date, dates.Item2.Date);

                for (DateTime date = dates.Item1.Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
                {
                    var currencyExchange = currencyExchanges.Where(x => x.Date.Month == date.Month && x.Date.Year == date.Year);

                    response.Data.Months.Add(new CurrencyExchangeModel
                    {
                        Month = date.Month,
                        Year = date.Year,
                        Description = DatesHelper.GetDateDescription(date),
                        Items = currencyExchange.Select(x => new CurrencyExchangeItemModel { CurrencyDesc = x.Currency.Text, Exchange = x.Exchange, CurrencyId = x.CurrencyId }).ToList(),
                    });
                }

                if (managementReport.Budgets.Any())
                {
                    var budgets = managementReport.Budgets.Select(x => new BudgetItem(x)).ToList();
                    response.Data.Budgets = new List<BudgetItem>();

                    if (budgets.Any(x => x.Description.ToUpper() == EnumBudgetType.budget))
                    {
                        var budget = budgets.Where(x => x.Description == EnumBudgetType.budget)
                                                 .OrderByDescending(x => x.StartDate).FirstOrDefault();

                        response.Data.Budgets.Add(budget);
                    }
                    if (budgets.Any(x => x.Description.ToUpper() == EnumBudgetType.pfa1))
                    {
                        var budget = budgets.Where(x => x.Description.ToUpper() == EnumBudgetType.pfa1)
                                .OrderByDescending(x => x.StartDate).FirstOrDefault();

                        response.Data.Budgets.Add(budget);
                    }
                    if (budgets.Any(x => x.Description.ToUpper() == EnumBudgetType.pfa2))
                    {
                        var budget = budgets.Where(x => x.Description.ToUpper() == EnumBudgetType.pfa2)
                                .OrderByDescending(x => x.StartDate).FirstOrDefault();

                        response.Data.Budgets.Add(budget);
                    }

                    if (budgets.Any(x => x.Description.ToUpper() == EnumBudgetType.Projected))
                    {
                        var budget = budgets.Where(x => x.Description.ToUpper() == EnumBudgetType.Projected)
                                .OrderByDescending(x => x.StartDate).FirstOrDefault();

                        response.Data.Budgets.Add(budget);
                    }

                    if (budgets.Any(x => x.Description.ToUpper() == EnumBudgetType.Real))
                    {
                        var budget = budgets.Where(x => x.Description.ToUpper() == EnumBudgetType.Real)
                                .OrderByDescending(x => x.StartDate).FirstOrDefault();

                        response.Data.Budgets.Add(budget);
                    }

                    response.Data.BudgetsHistory = budgets.OrderBy(x => x.StartDate).ToList();

                    response.Data.BudgetTotal = response.Data.Budgets.Sum(x => x.Value);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }

            return response;
        }

        public Response<CostDetailStaffMonthModel> GetCostDetailMonth(int id, int month, int year)
        {
            var response = new Response<CostDetailStaffMonthModel> { Data = new CostDetailStaffMonthModel() };

            var managementReport = unitOfWork.ManagementReportRepository.GetById(id);
            this.VerifyAnalyticMonths(managementReport, managementReport.StartDate, managementReport.EndDate);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            var monthYear = new DateTime(year, month, 1);

            CostDetail costDetail = unitOfWork.CostDetailRepository.GetByManagementReportAndMonthYear(id, monthYear);

            var lastType = costDetail.CostDetailStaff
                                        .Where(x => x.BudgetType.Name != EnumBudgetType.Projected)
                                        .OrderByDescending(x => x.BudgetTypeId)
                                        .FirstOrDefault();

            string typeBudget = EnumBudgetType.Real;

            //if (lastType != null)
            //{
            //    typeBudget = lastType.BudgetType.Name.ToUpper();
            //}

            //if (costDetail.HasReal)
            //{
            //    typeBudget = EnumBudgetType.Real;
            //}

            var allocations = unitOfWork.AllocationRepository.GetAllocationsBetweenDay(monthYear);

            var subcategories = new List<CostSubcategoryMonth>();
            var resources = new List<CostMonthEmployeeStaff>();

            if (costDetail != null)
            {
                var costDetailStaff = costDetail.CostDetailStaff.Where(x => x.BudgetType.Name == typeBudget).ToList();
                subcategories = this.Translate(costDetailStaff);
                resources = this.Translate(costDetail.CostDetailResources.Where(x => x.BudgetType.Name == typeBudget).ToList(), monthYear, allocations, managementReport.AnalyticId);

                //if (!getReal)
                //{
                //    foreach (var sub in subcategories)
                //    {
                //        sub.CostDetailStaffId = 0;
                //    }

                //    foreach (var employee in resources)
                //    {
                //        employee.Id = 0;
                //    }
                //}

                var employees = resources.Select(x => x.EmployeeId).ToList();

                response.Data.Id = costDetail.Id;
                response.Data.TotalProvisioned = costDetail.TotalProvisioned;

                if (costDetail.CostDetailResources != null)
                {
                    var socialCharges = unitOfWork.RrhhRepository.GetSocialCharges(year, month, employees);

                    response.Data.SocialCharges = new List<SocialChargeModelItem>();

                    foreach (var socialCharge in socialCharges)
                    {
                        foreach (var socialChargeItem in socialCharge.Items)
                        {
                            if (socialChargeItem.AccountNumber == 648001 || socialChargeItem.AccountNumber == 960 || socialChargeItem.AccountNumber == 930 || socialChargeItem.AccountNumber == 962) continue;

                            var item = new SocialChargeModelItem();

                            item.Employee = socialCharge.Employee?.Name;
                            item.EmployeeNumber = socialCharge.Employee?.EmployeeNumber;
                            item.AccountName = socialChargeItem.AccountName;
                            item.AccountNumber = socialChargeItem.AccountNumber;
                            item.Value = CryptographyHelper.Decrypt(socialChargeItem.Value);
                            item.Year = year;
                            item.Month = month;

                            var resourceAllocation = allocations.SingleOrDefault(x => x.EmployeeId == socialCharge.EmployeeId && x.AnalyticId == managementReport.AnalyticId && x.StartDate.Date == monthYear);

                            if (resourceAllocation != null)
                            {
                                if (!decimal.TryParse(item.Value, out var valueDecimal))
                                {
                                    item.Value = "0";
                                }
                                else
                                {
                                    valueDecimal *= resourceAllocation.RealPercentage / 100;
                                    item.Value = valueDecimal.ToString(CultureInfo.InvariantCulture);
                                }
                            }

                            response.Data.SocialCharges.Add(item);
                        }
                    }
                }
            }

            response.Data.ManagementReportId = id;
            response.Data.MonthYear = monthYear;
            response.Data.Employees = resources.OrderBy(x => x.Name).ToList();
            response.Data.Subcategories = subcategories.Where(x => x.Name != EnumCostDetailType.AjusteGeneral).OrderBy(x => x.NameCategory).ThenBy(x => x.Name).ToList();

            return response;
        }

        public Response<CostDetailStaffModel> GetCostDetailStaff(int ManagementReportId)
        {
            var response = new Response<CostDetailStaffModel> { Data = new CostDetailStaffModel() };
            try
            {
                var managementReport = unitOfWork.ManagementReportRepository.GetById(ManagementReportId);
                if (managementReport == null)
                {
                    response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                    return response;
                }

                response.Data.AnalyticId = managementReport.Analytic.Id;

                response.Data.MonthsHeader = new List<MonthHeaderCost>();
                response.Data.ManagementReportId = managementReport.Analytic.ManagementReport.Id;

                //Obtengo los meses que tiene la analitica
                var dates = SetDates(managementReport.Analytic);

                var costDetails = managementReport.CostDetails;

                for (DateTime date = new DateTime(dates.Item1.Year, dates.Item1.Month, 1).Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
                {
                    var monthHeader = new MonthHeaderCost();
                    monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                    monthHeader.MonthYear = date;
                    monthHeader.Month = date.Month;
                    monthHeader.Year = date.Year;

                    var costDetailMonth = costDetails.FirstOrDefault(x => x.MonthYear.Date == date.Date);

                    if (costDetailMonth != null)
                    {
                        monthHeader.CostDetailId = costDetailMonth.Id;
                        monthHeader.Closed = costDetailMonth.Closed;
                    }

                    response.Data.MonthsHeader.Add(monthHeader);
                }

                var allCategoriesData = this.FillCategoriesByMonth(response.Data.MonthsHeader, costDetails);

                List<string> infraRed = new List<string>(new string[] { EnumCostDetailType.Red, EnumCostDetailType.Infraestructura });

                response.Data.BudgetTypes = unitOfWork.ManagementReportRepository.GetTypesBudget().Select(x => new BudgetTypeItem(x)).ToList();
                response.Data.AllSubcategories = this.FillAllSubcategories();
                response.Data.CostCategories = allCategoriesData.Where(r => r.Show == true && r.BelongEmployee == false && !infraRed.Any(y => r.Name == y)).OrderBy(r => r.Name).ToList();
                response.Data.CostCategoriesRedInfra = allCategoriesData.Where(x => x.Show == true && infraRed.Any(y => x.Name == y)).ToList();
                response.Data.OtherCategories = allCategoriesData.Where(r => r.Show == false && r.BelongEmployee == false).OrderBy(r => r.Name).ToList();
                response.Data.CostEmployees = managementReportService.FillCostEmployeesByMonth(managementReport.Analytic.Id, response.Data.MonthsHeader, costDetails, dates.Item1, dates.Item2);
                response.Data.CostCategoriesEmployees = allCategoriesData.Where(r => r.BelongEmployee == true).OrderBy(r => r.Name).ToList();
                response.Data.Status = managementReport.Status;
                response.Data.State = managementReport.State == null ? EnumBudgetType.budget.ToLower() : managementReport.State.Name.ToLower();

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response SaveCostDetailStaff(CostDetailStaffModel pDetailCost)
        {
            var response = new Response();
            try
            {
                var managementReport = unitOfWork.ManagementReportRepository.GetById(pDetailCost.ManagementReportId);
                var analytic = unitOfWork.AnalyticRepository.GetById(managementReport.AnalyticId);
                var budgetTypes = unitOfWork.ManagementReportRepository.GetTypesBudget();

                var listMonths = this.VerifyAnalyticMonths(managementReport, analytic.StartDateContract, analytic.EndDateContract);

                // var costDetails = unitOfWork.CostDetailRepository.GetByManagementReport(pDetailCost.ManagementReportId);
                var costDetails = managementReport.CostDetails.ToList();

                var allCostCategories = pDetailCost.CostCategories
                                                        .Union(pDetailCost.CostCategoriesEmployees)
                                                        .Union(pDetailCost.CostCategoriesRedInfra)
                                                        .ToList();

                this.InsertUpdateCostDetailStaff(allCostCategories, costDetails);

                if (pDetailCost.CostEmployees.Count > 0)
                {
                    IList<CostResourceEmployee> EmployeesWithAllMonths;

                    //Verifico si la fecha final del proyecto es la misma que la de final de la analitica
                    if (new DateTime(managementReport.EndDate.Year, managementReport.EndDate.Month, 1).Date == new DateTime(analytic.EndDateContract.Year, analytic.EndDateContract.Month, 1).Date)
                    {
                        EmployeesWithAllMonths = pDetailCost.CostEmployees;
                    }
                    else
                    {
                        EmployeesWithAllMonths = managementReportService.AddAnalyticMonthsToEmployees(pDetailCost.CostEmployees, managementReport.Id, analytic.StartDateContract, analytic.EndDateContract);
                    }

                    managementReportService.InsertUpdateCostDetailResources(EmployeesWithAllMonths, managementReport.CostDetails.ToList(), managementReport.Id);
                }

                this.InsertTotalBudgets(pDetailCost, pDetailCost.ManagementReportId);

                //Cambiar estado de budget
                if (pDetailCost.CloseState)
                {
                    BudgetType nextState = budgetTypes.Where(x => x.Name == EnumBudgetType.pfa1).FirstOrDefault();
                    if (managementReport.State != null)
                    {
                        switch (managementReport.State.Name.ToUpper())
                        {
                            case EnumBudgetType.budget:
                                nextState = budgetTypes.Where(x => x.Name == EnumBudgetType.pfa1).FirstOrDefault();
                                break;
                            case EnumBudgetType.pfa1:
                                nextState = budgetTypes.Where(x => x.Name == EnumBudgetType.pfa2).FirstOrDefault();
                                break;
                            case EnumBudgetType.pfa2:
                                nextState = budgetTypes.Where(x => x.Name == EnumBudgetType.Real).FirstOrDefault();
                                break;
                        }
                    }

                    managementReport.State = nextState;
                    managementReport.StateGenerated = false;
                    unitOfWork.ManagementReportRepository.UpdateState(managementReport);
                    unitOfWork.ManagementReportRepository.UpdateStateGenerated(managementReport);
                }
                else
                {
                    //Si no estoy finalizando el estado pero mi estado actual es nulo pasarlo a budget
                    if (managementReport.State == null)
                    {
                        managementReport.StateId = budgetTypes.Where(x => x.Name == EnumBudgetType.budget).FirstOrDefault().Id;
                        unitOfWork.ManagementReportRepository.UpdateState(managementReport);
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

        public Response SaveCostDetailStaffMonth(CostDetailStaffMonthModel pMonthDetail)
        {
            var response = new Response();
            CostDetailModel _detailModel = new CostDetailModel();
            _detailModel.ManagementReportId = pMonthDetail.ManagementReportId;
            _detailModel.CostEmployees = new List<CostResourceEmployee>();
            decimal totalSalary = 0;

            var listCategories = new List<CostCategory>();
            var costCategory = new CostCategory();
            costCategory.MonthsCategory = new List<MonthDetailCostStaff>();
            var monthStaff = new MonthDetailCostStaff();

            var budgetTypes = unitOfWork.ManagementReportRepository.GetTypesBudget();
            var costDetails = unitOfWork.CostDetailRepository.GetByManagementReport(pMonthDetail.ManagementReportId);

            try
            {
                if (pMonthDetail.Employees != null)
                {
                    foreach (var employee in pMonthDetail.Employees)
                    {
                        var cost = new CostResourceEmployee();
                        cost.MonthsCost = new List<MonthDetailCost>();
                        MonthDetailCost month = new MonthDetailCost();

                        cost.EmployeeId = employee.EmployeeId;
                        cost.UserId = employee.UserId;
                        month.MonthYear = pMonthDetail.MonthYear;

                        month.Real.Id = employee.Id;
                        month.Real.Value = employee.Salary;
                        month.Real.Charges = employee.Charges;
                        month.Real.Bono = employee.Bono;
                        month.Real.BudgetTypeId = budgetTypes.Where(x => x.Name == EnumBudgetType.Real).FirstOrDefault().Id;

                        totalSalary += employee.Salary ?? 0;
                        totalSalary += employee.Charges ?? 0;

                        cost.MonthsCost.Add(month);
                        _detailModel.CostEmployees.Add(cost);
                    }
                    managementReportService.InsertUpdateCostDetailResources(_detailModel.CostEmployees, costDetails, pMonthDetail.ManagementReportId, true);
                }

                if (pMonthDetail.Subcategories != null)
                {
                    foreach (var item in pMonthDetail.Subcategories)
                    {
                        var subcategory = new CostSubcategory();

                        monthStaff.MonthYear = pMonthDetail.MonthYear;

                        subcategory.Id = item.Id;
                        subcategory.CostDetailStaffId = item.CostDetailStaffId;
                        subcategory.Description = item.Description;
                        subcategory.Value = item.Value;
                        subcategory.BudgetTypeId = budgetTypes.Where(x => x.Name == EnumBudgetType.Real).FirstOrDefault().Id;
                        subcategory.CurrencyId = item.CurrencyId == 0 ? appSetting.CurrencyPesos : item.CurrencyId;
                        subcategory.Deleted = item.Deleted;

                        monthStaff.SubcategoriesReal.Add(subcategory);
                    }

                    costCategory.MonthsCategory.Add(monthStaff);
                    listCategories.Add(costCategory);

                    this.InsertUpdateCostDetailStaff(listCategories, costDetails);
                }

                var costDetailMonth = costDetails.SingleOrDefault(x => x.MonthYear.Date == pMonthDetail.MonthYear.Date);

                if (costDetailMonth != null)
                {
                    costDetailMonth.TotalProvisioned = pMonthDetail.TotalProvisioned ?? pMonthDetail.TotalProvisioned.GetValueOrDefault();

                    unitOfWork.CostDetailRepository.UpdateTotals(costDetailMonth);
                }

                //this.InsertTotalSalaryStaffReport(pMonthDetail.ManagementReportId, totalSalary, pMonthDetail.MonthYear);

                if (costDetailMonth.HasReal == false)
                {
                    costDetailMonth.HasReal = true;
                    unitOfWork.CostDetailRepository.UpdateHasReal(costDetailMonth);
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

        public Response Close(ManagementReportCloseModel model)
        {
            var response = new Response();

            var detailCost = unitOfWork.CostDetailRepository.Get(model.DetailCostId);

            if (detailCost == null) response.AddError(Resources.ManagementReport.CostDetail.NotFound);

            if (!model.Date.HasValue) response.AddError(Resources.ManagementReport.ManagementReport.DateRequired);

            if (response.HasErrors()) return response;

            var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            if (model.Date.GetValueOrDefault().Date >= currentMonth.Date)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.CannotClosed);
                return response;
            }

            try
            {
                detailCost.Closed = true;

                unitOfWork.CostDetailRepository.Close(detailCost);

                unitOfWork.Save();

                response.AddSuccess(Resources.ManagementReport.ManagementReport.ClosedSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            try
            {
                if (unitOfWork.ManagementReportRepository.AllMonthsAreClosed(detailCost.ManagementReportId))
                {
                    var managementReport = new Domain.Models.ManagementReport.ManagementReport
                    {
                        Id = detailCost.ManagementReportId,
                        Status = ManagementReportStatus.Closed
                    };

                    unitOfWork.ManagementReportRepository.UpdateStatus(managementReport);
                    unitOfWork.Save();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }

            return response;
        }

        public Response GetCategories()
        {
            var response = new Response<List<CostDetailCategories>> { Data = new List<CostDetailCategories>() };

            try
            {
                var Allcategories = unitOfWork.CostDetailRepository.GetCategories();
                var categories = Allcategories.Where(x =>
                                                        x.Name != EnumCostDetailType.InformeFinal.ToString() &&
                                                        x.Name != EnumCostDetailType.AjusteGeneral.ToString() &&
                                                        x.Name != EnumCostDetailType.Profile.ToString() &&
                                                        x.Name != EnumCostDetailType.Recursos.ToString()
                                                    ).ToList();

                response.Data = categories;
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response GeneratePFA(int ManagementReportId, string PFA)
        {
            var response = new Response<CostDetailStaffModel> { Data = new CostDetailStaffModel() };
            try
            {
                var managementReport = unitOfWork.ManagementReportRepository.GetById(ManagementReportId);
                if (managementReport == null)
                {
                    response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                    return response;
                }

                var typesBudgets = unitOfWork.ManagementReportRepository.GetTypesBudget().Select(x => new BudgetTypeItem(x)).ToList();
                int idPFA;
                DateTime dateToCompare;

                if (PFA == EnumBudgetType.pfa1)
                {
                    idPFA = typesBudgets.Where(x => x.Name == EnumBudgetType.pfa1).FirstOrDefault().Id;
                    dateToCompare = new DateTime(2019, 4, 1);
                }
                else
                {
                    idPFA = typesBudgets.Where(x => x.Name == EnumBudgetType.pfa2).FirstOrDefault().Id;
                    dateToCompare = new DateTime(2019, 9, 1);
                }

                var costDetails = managementReport.CostDetails;

                //Obtengo los meses que tiene la analitica
                var dates = this.SetDates(managementReport.Analytic);
                var Months = new List<MonthDetailCostStaff>();

                for (DateTime date = new DateTime(dates.Item1.Year, dates.Item1.Month, 1).Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
                {
                    var monthHeader = new MonthDetailCostStaff();
                    monthHeader.MonthYear = date;

                    Months.Add(monthHeader);
                }

                foreach (var mounth in Months)
                {
                    var costDetailMonth = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date).FirstOrDefault();

                    //Elimino todos los registros del PFA ya guardados para ese mes
                    unitOfWork.CostDetailStaffRepository.Delete(costDetailMonth.CostDetailStaff.Where(x => x.BudgetTypeId == idPFA).ToList());
                    unitOfWork.CostDetailResourceRepository.Delete(costDetailMonth.CostDetailResources.Where(x => x.BudgetTypeId == idPFA).ToList());

                    if (costDetailMonth != null)
                    {
                        List<CostDetailStaff> subcategories;
                        List<CostDetailResource> employees;

                        if (mounth.MonthYear.Date < dateToCompare.Date)
                        {
                            subcategories = costDetailMonth.CostDetailStaff.Where(x => x.BudgetTypeId == typesBudgets.FirstOrDefault(t => t.Name == EnumBudgetType.Real).Id).ToList();
                            employees = costDetailMonth.CostDetailResources.Where(x => x.BudgetTypeId == typesBudgets.FirstOrDefault(t => t.Name == EnumBudgetType.Real).Id).ToList();
                        }
                        else
                        {
                            if (PFA == EnumBudgetType.pfa1)
                            {
                                subcategories = costDetailMonth.CostDetailStaff.Where(x => x.BudgetTypeId == typesBudgets.FirstOrDefault(t => t.Name == EnumBudgetType.budget).Id).ToList();
                                employees = costDetailMonth.CostDetailResources.Where(x => x.BudgetTypeId == typesBudgets.FirstOrDefault(t => t.Name == EnumBudgetType.budget).Id).ToList();
                            }
                            else
                            {
                                subcategories = costDetailMonth.CostDetailStaff.Where(x => x.BudgetTypeId == typesBudgets.FirstOrDefault(t => t.Name == EnumBudgetType.pfa1).Id).ToList();
                                employees = costDetailMonth.CostDetailResources.Where(x => x.BudgetTypeId == typesBudgets.FirstOrDefault(t => t.Name == EnumBudgetType.pfa1).Id).ToList();
                            }
                        }

                        if (subcategories != null)
                        {
                            foreach (var subcategory in subcategories)
                            {
                                var entity = new CostDetailStaff();

                                entity.Value = subcategory.Value ?? 0;
                                entity.Description = subcategory.Description;
                                entity.CostDetailId = costDetailMonth.Id;
                                entity.CostDetailSubcategoryId = subcategory.CostDetailSubcategoryId;
                                entity.CurrencyId = subcategory.CurrencyId;
                                entity.BudgetTypeId = idPFA;

                                unitOfWork.CostDetailStaffRepository.Insert(entity);
                            }
                        }

                        if (employees != null)
                        {
                            foreach (var employee in employees)
                            {
                                var entity = new CostDetailResource();

                                entity.CostDetailId = costDetailMonth.Id;
                                entity.Value = employee.Value;
                                entity.Adjustment = employee.Adjustment ?? 0;
                                entity.Charges = employee.Charges;
                                entity.EmployeeId = employee.EmployeeId;
                                entity.UserId = employee?.UserId;
                                entity.BudgetTypeId = idPFA;

                                unitOfWork.CostDetailResourceRepository.Insert(entity);
                            }
                        }
                    }
                }

                managementReport.StateGenerated = true;
                unitOfWork.ManagementReportRepository.UpdateStateGenerated(managementReport);

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

        public Response Reset(int id)
        {
            var response = new Response();

            var managementReport = unitOfWork.ManagementReportRepository.Get(id);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            try
            {
                var typesBudgets = unitOfWork.ManagementReportRepository.GetTypesBudget().Select(x => new BudgetTypeItem(x)).ToList();
                managementReport.StateId = typesBudgets.FirstOrDefault(x => x.Name == EnumBudgetType.budget)?.Id;
                managementReport.StateGenerated = false;
                unitOfWork.ManagementReportRepository.UpdateState(managementReport);
                unitOfWork.ManagementReportRepository.UpdateStateGenerated(managementReport);
                unitOfWork.Save();

                response.AddSuccess(Resources.Common.SaveSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void InsertUpdateCostDetailStaff(List<CostCategory> costCategories, IList<CostDetail> costDetails)
        {
            try
            {
                foreach (var category in costCategories)
                {
                    foreach (var month in category.MonthsCategory)
                    {
                        var allSubcategories = month.SubcategoriesBudget
                                                        .Union(month.SubcategoriesPfa1)
                                                        .Union(month.SubcategoriesPfa2)
                                                        .Union(month.SubcategoriesReal)
                                                        .Union(month.SubcategoriesProjected);

                        foreach (var subCatBudget in allSubcategories)
                        {
                            var entity = new CostDetailStaff();

                            if (subCatBudget.CostDetailStaffId > 0)
                            {
                                entity = unitOfWork.CostDetailStaffRepository.Get(subCatBudget.CostDetailStaffId);
                                if (subCatBudget.Deleted == true)
                                {
                                    unitOfWork.CostDetailStaffRepository.Delete(entity);
                                }
                                else
                                {
                                    if (subCatBudget.Value != entity.Value || subCatBudget.Description != entity.Description)
                                    {
                                        entity.Value = subCatBudget.Value ?? 0;
                                        entity.Description = subCatBudget.Description;
                                        entity.CurrencyId = subCatBudget.CurrencyId.GetValueOrDefault();
                                        entity.OriginalValue = subCatBudget.OriginalValue ?? 0;

                                        unitOfWork.CostDetailStaffRepository.Update(entity);
                                    }
                                }
                            }
                            else
                            {
                                entity.Value = subCatBudget.Value ?? 0;
                                entity.OriginalValue = subCatBudget.OriginalValue ?? 0;
                                entity.Description = subCatBudget.Description;
                                entity.CurrencyId = subCatBudget.CurrencyId.GetValueOrDefault();
                                entity.CostDetailId = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == month.MonthYear.Date).FirstOrDefault().Id;
                                entity.CostDetailSubcategoryId = subCatBudget.Id;
                                entity.BudgetTypeId = subCatBudget.BudgetTypeId;

                                unitOfWork.CostDetailStaffRepository.Insert(entity);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                throw ex;
            }
        }

        private bool CheckRoles(Analytic analytic, Response<ManagementReportStaffDetail> response)
        {
            var currentUser = userData.GetCurrentUser();

            if (roleManager.IsCdg() || roleManager.IsDafOrGaf())
            {
                return true;
            }
            else
            {
                var analyticsByDelegates = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.ManagementReport);

                if ((roleManager.IsManager() && currentUser.Id == analytic.ManagerId.GetValueOrDefault()) ||
                    (roleManager.IsDirector() && analytic.Sector.ResponsableUserId == currentUser.Id) ||
                    analyticsByDelegates.Any(x => x.AnalyticSourceId == analytic.Id))
                {
                    if (response != null)
                    {
                        if (analyticsByDelegates.Any(x => x.AnalyticSourceId == analytic.Id))
                        {
                            response.Data.DelegateId = currentUser.Id;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private List<CostMonthEmployeeStaff> Translate(List<CostDetailResource> resources, DateTime monthYear,
            IList<Allocation> allocations, int managementReportAnalyticId)
        {
            var list = new List<CostMonthEmployeeStaff>();

            foreach (var resource in resources)
            {
                var item = new CostMonthEmployeeStaff();

                item.Id = resource.Id;

                item.EmployeeId = resource.EmployeeId;

                var employee = unitOfWork.EmployeeRepository.GetWithSocialChargesAndAllocations(resource.EmployeeId);
                item.Name = employee?.Name;
                item.UserId = resource.UserId;
                item.MonthYear = monthYear;

                if (string.IsNullOrWhiteSpace(resource.Value))
                {
                    var socialCharge = employee?.SocialCharges.FirstOrDefault(x => x.Year == monthYear.Year && x.Month == monthYear.Month);

                    if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.SalaryTotal), out var salary)) salary = 0;
                    if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.ChargesTotal), out var charges)) charges = 0;

                    item.Salary = salary;
                    item.Charges = charges;
                    item.Total = salary + charges;

                    if (salary > 0)
                    {
                        item.ChargesPercentage = (charges / salary) * 100;
                    }

                    if (socialCharge != null)
                    {
                        //641300 codigo gratificaciones
                        var bono = unitOfWork.RrhhRepository.GetSocialChargeItem(641300, socialCharge.Id);

                        if (bono != null)
                        {
                            if (!decimal.TryParse(CryptographyHelper.Decrypt(bono?.Value),
                                out var bonoValue)) bonoValue = 0;

                            item.Bono = bonoValue;
                            item.Total += bonoValue;
                        }
                    }
                }
                else
                {
                    if (!decimal.TryParse(CryptographyHelper.Decrypt(resource.Value), out var salary)) salary = 0;
                    if (!decimal.TryParse(CryptographyHelper.Decrypt(resource.Charges), out var charges)) charges = 0;
                    if (!decimal.TryParse(CryptographyHelper.Decrypt(resource.Bono), out var bono)) bono = 0;

                    item.Salary = salary;
                    item.Charges = charges;
                    item.Bono = bono;
                    item.Total = salary + charges;

                    if (salary > 0)
                    {
                        item.ChargesPercentage = ((charges + bono) / salary) * 100;
                    }
                }

                list.Add(item);
            }

            foreach (var allocation in allocations)
            {
                if (allocation.AnalyticId == managementReportAnalyticId && list.All(x => x.EmployeeId != allocation.EmployeeId))
                {
                    var item = new CostMonthEmployeeStaff();
                    item.EmployeeId = allocation.EmployeeId;
                    item.Id = 0;
                    item.Name = allocation.Employee?.Name;
                    item.MonthYear = monthYear;

                    var user = unitOfWork.UserRepository.GetByEmail(allocation.Employee?.Email);

                    if (user != null)
                        item.UserId = user.Id;

                    if (allocation.Employee != null)
                    {
                        var socialCharge = allocation.Employee.SocialCharges.FirstOrDefault(x => x.Year == allocation.StartDate.Year && x.Month == allocation.StartDate.Month);

                        if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.SalaryTotal), out var salary)) salary = 0;
                        if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.ChargesTotal), out var charges)) charges = 0;

                        var newValueCharges = (allocation.RealPercentage / 100) * charges;
                        var newValueSalary = salary * (allocation.RealPercentage / 100);

                        item.Salary = newValueSalary;
                        item.Charges = newValueCharges;
                        item.Total = newValueSalary + newValueCharges;

                        //641300 codigo gratificaciones
                        var bono = unitOfWork.RrhhRepository.GetSocialChargeItem(641300, socialCharge.Id);

                        if (bono != null)
                        {
                            if (!decimal.TryParse(CryptographyHelper.Decrypt(bono?.Value),
                                out var bonoValue)) bonoValue = 0;

                            item.Bono = (allocation.RealPercentage / 100) * bonoValue;
                            item.Total += item.Bono;
                        }

                        if (salary > 0)
                        {
                            item.ChargesPercentage = ((newValueCharges + item.Bono) / newValueSalary) * 100;
                        }
                    }

                    list.Add(item);
                }
            }

            return list;
        }

        private List<CostCategory> FillCategoriesByMonth(IList<MonthHeaderCost> Months, ICollection<CostDetail> costDetails)
        {
            List<CostCategory> costCategories = new List<CostCategory>();

            var Allcategories = unitOfWork.CostDetailRepository.GetCategories();
            var categories = Allcategories.Where(x =>
                                                    x.Name != EnumCostDetailType.InformeFinal.ToString() &&
                                                    x.Name != EnumCostDetailType.Recursos.ToString()
                                                ).ToList();

            foreach (var category in categories)
            {
                var detailCategory = new CostCategory();
                detailCategory.MonthsCategory = new List<MonthDetailCostStaff>();
                bool hasValue = false;

                detailCategory.Id = category.Id;
                detailCategory.Name = category.Name;
                detailCategory.BelongEmployee = category.BelongEmployee;
                detailCategory.Default = category.Default;
                detailCategory.TypeName = category.Name;

                foreach (var mounth in Months)
                {
                    var monthDetail = new MonthDetailCostStaff();
                    monthDetail.SubcategoriesBudget = new List<CostSubcategory>();
                    monthDetail.SubcategoriesPfa1 = new List<CostSubcategory>();
                    monthDetail.SubcategoriesPfa2 = new List<CostSubcategory>();
                    monthDetail.SubcategoriesReal = new List<CostSubcategory>();
                    monthDetail.SubcategoriesProjected = new List<CostSubcategory>();

                    monthDetail.Display = mounth.Display;
                    monthDetail.MonthYear = mounth.MonthYear;
                    monthDetail.Year = mounth.MonthYear.Year;
                    monthDetail.Month = mounth.MonthYear.Month;
                    monthDetail.Closed = mounth.Closed;

                    var costDetailMonth = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date).FirstOrDefault();
                    if (costDetailMonth != null)
                    {
                        monthDetail.CostDetailId = costDetailMonth.Id;

                        var subcategories = costDetailMonth.CostDetailStaff.Where(o => o.CostDetailSubcategory.CostDetailCategoryId == category.Id).ToList();
                        if (subcategories != null)
                        {
                            foreach (var subcategory in subcategories)
                            {
                                var detailSubcategory = new CostSubcategory();

                                detailSubcategory.Id = subcategory.CostDetailSubcategoryId;
                                detailSubcategory.CostDetailStaffId = subcategory.Id;
                                detailSubcategory.Name = subcategory.CostDetailSubcategory.Name;
                                detailSubcategory.Description = subcategory.Description;
                                detailSubcategory.Value = subcategory.Value;
                                detailSubcategory.OriginalValue = subcategory.OriginalValue;
                                detailSubcategory.CurrencyId = subcategory.CurrencyId;
                                detailSubcategory.BudgetTypeId = subcategory.BudgetTypeId;

                                if (detailSubcategory.Value > 0)
                                {
                                    hasValue = true;
                                }

                                switch (subcategory.BudgetType.Name.ToUpper())
                                {
                                    case EnumBudgetType.budget:
                                        monthDetail.SubcategoriesBudget.Add(detailSubcategory);
                                        break;
                                    case EnumBudgetType.pfa1:
                                        monthDetail.SubcategoriesPfa1.Add(detailSubcategory);
                                        break;
                                    case EnumBudgetType.pfa2:
                                        monthDetail.SubcategoriesPfa2.Add(detailSubcategory);
                                        break;
                                    case EnumBudgetType.Real:
                                        monthDetail.SubcategoriesReal.Add(detailSubcategory);
                                        break;
                                    case EnumBudgetType.Projected:
                                        monthDetail.SubcategoriesProjected.Add(detailSubcategory);
                                        break;
                                }
                            }

                            monthDetail.TotalBudget = monthDetail.SubcategoriesBudget.Sum(x => x.Value) ?? 0;
                            monthDetail.TotalPfa1 = monthDetail.SubcategoriesPfa1.Sum(x => x.Value) ?? 0;
                            monthDetail.TotalPfa2 = monthDetail.SubcategoriesPfa2.Sum(x => x.Value) ?? 0;
                            monthDetail.TotalReal = monthDetail.SubcategoriesReal.Sum(x => x.Value) ?? 0;
                            monthDetail.TotalProjected = monthDetail.SubcategoriesProjected.Sum(x => x.Value) ?? 0;
                        }
                    }

                    if (category.Default == true || hasValue)
                    {
                        detailCategory.Show = true;
                    }

                    detailCategory.MonthsCategory.Add(monthDetail);
                }

                costCategories.Add(detailCategory);
            }

            return costCategories;
        }

        private List<CostSubcategory> FillAllSubcategories()
        {
            var subcategories = unitOfWork.CostDetailRepository.GetSubcategories();

            return subcategories
                        .Select(x => new CostSubcategory
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IdCategory = x.CostDetailCategoryId
                        })
                        .OrderBy(x => x.Name)
                        .ToList();
        }

        private Tuple<DateTime, DateTime> SetDates(Analytic analytic)
        {
            var today = DateTime.UtcNow;

            DateTime startDate;
            DateTime endDate;

            if (analytic.ManagementReport != null)
            {
                startDate = new DateTime(analytic.ManagementReport.StartDate.Year, analytic.ManagementReport.StartDate.Month, 1);
                endDate = new DateTime(analytic.ManagementReport.EndDate.Year, analytic.ManagementReport.EndDate.Month,
                    DateTime.DaysInMonth(analytic.ManagementReport.EndDate.Year,
                        analytic.ManagementReport.EndDate.Month));
            }
            else
            {
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
            }

            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        private List<MonthDetailCost> VerifyAnalyticMonths(Sofco.Domain.Models.ManagementReport.ManagementReport pManagementReport, DateTime startDateAnalytic, DateTime endDateAnalytic)
        {
            List<MonthDetailCost> MonthsAnalytic = new List<MonthDetailCost>();
            try
            {
                for (DateTime date = new DateTime(startDateAnalytic.Year, startDateAnalytic.Month, 1).Date; date.Date <= endDateAnalytic.Date; date = date.AddMonths(1))
                {
                    var month = new MonthDetailCost();
                    month.MonthYear = date;

                    MonthsAnalytic.Add(month);
                }

                // Verifico que todos los meses de la analitica esten cargados en base de datos.

                foreach (var mounth in MonthsAnalytic)
                {
                    var costDetailMonth = pManagementReport.CostDetails
                                                    .Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date)
                                                    .FirstOrDefault();
                    if (costDetailMonth == null)
                    {
                        var entity = new CostDetail();

                        entity.ManagementReportId = pManagementReport.Id;
                        entity.MonthYear = mounth.MonthYear;

                        unitOfWork.CostDetailRepository.Insert(entity);
                    }
                }

                unitOfWork.Save();

                return MonthsAnalytic;
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                throw ex;
            }
        }

        private void InsertTotalBudgets(CostDetailStaffModel costDetail, int managementReportId)
        {
            decimal totalBudget = 0;
            decimal totalPFA1 = 0;
            decimal totalPFA2 = 0;
            decimal totalProyected = 0;

            try
            {
                foreach (var monthHeader in costDetail.MonthsHeader)
                {
                    totalBudget += monthHeader.Budget.TotalCost ?? 0;
                    totalPFA1 += monthHeader.Pfa1.TotalCost ?? 0;
                    totalPFA2 += monthHeader.Pfa2.TotalCost ?? 0;
                    totalProyected += monthHeader.Projected.TotalCost ?? 0;
                }

                var currentUser = userData.GetCurrentUser();
                var budgestDB = unitOfWork.ManagementReportRepository.GetBudgetByIdStaff(managementReportId);
                decimal lastBudget = 0;
                decimal lastPfa1 = 0;
                decimal lastPfa2 = 0;
                decimal lastProyected = 0;

                if (budgestDB.Any())
                {
                    lastBudget = budgestDB.Any(x => x.Description.ToUpper() == EnumBudgetType.budget) ?
                                    budgestDB.Where(x => x.Description.ToUpper() == EnumBudgetType.budget)
                                                .OrderByDescending(x => x.StartDate)
                                                .FirstOrDefault().Value : 0;
                    lastPfa1 = budgestDB.Any(x => x.Description.ToUpper() == EnumBudgetType.pfa1) ?
                                budgestDB.Where(x => x.Description.ToUpper() == EnumBudgetType.pfa1)
                                            .OrderByDescending(x => x.StartDate)
                                            .FirstOrDefault().Value : 0;
                    lastPfa2 = budgestDB.Any(x => x.Description.ToUpper() == EnumBudgetType.pfa2) ?
                                budgestDB.Where(x => x.Description.ToUpper() == EnumBudgetType.pfa2)
                                            .OrderByDescending(x => x.StartDate)
                                            .FirstOrDefault().Value : 0;
                    lastProyected = budgestDB.Any(x => x.Description.ToUpper() == EnumBudgetType.Projected) ?
                               budgestDB.Where(x => x.Description.ToUpper() == EnumBudgetType.Projected)
                                           .OrderByDescending(x => x.StartDate)
                                           .FirstOrDefault().Value : 0;
                }

                if (lastBudget != totalBudget)
                {

                    var budget = new Budget
                    {
                        StartDate = DateTime.Now,
                        ManagementReportId = managementReportId,
                        ModifiedBy = currentUser.UserName,
                        Value = totalBudget,
                        LastValue = lastBudget,
                        Description = EnumBudgetType.budget
                    };
                    unitOfWork.ManagementReportRepository.AddBudget(budget);
                }

                if (lastPfa1 != totalPFA1)
                {
                    var pfa1 = new Budget
                    {
                        StartDate = DateTime.Now,
                        ManagementReportId = managementReportId,
                        ModifiedBy = currentUser.UserName,
                        Value = totalPFA1,
                        LastValue = lastPfa1,
                        Description = EnumBudgetType.pfa1
                    };
                    unitOfWork.ManagementReportRepository.AddBudget(pfa1);
                }

                if (lastPfa2 != totalPFA2)
                {
                    var pfa2 = new Budget
                    {
                        StartDate = DateTime.Now,
                        ManagementReportId = managementReportId,
                        ModifiedBy = currentUser.UserName,
                        Value = totalPFA2,
                        LastValue = totalPFA2,
                        Description = EnumBudgetType.pfa2
                    };
                    unitOfWork.ManagementReportRepository.AddBudget(pfa2);
                }

                if (lastProyected != totalProyected)
                {
                    var proyected = new Budget
                    {
                        StartDate = DateTime.Now,
                        ManagementReportId = managementReportId,
                        ModifiedBy = currentUser.UserName,
                        Value = totalProyected,
                        LastValue = lastProyected,
                        Description = EnumBudgetType.Projected
                    };
                    unitOfWork.ManagementReportRepository.AddBudget(proyected);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                throw ex;
            }
        }

        private List<CostSubcategoryMonth> Translate(List<CostDetailStaff> staffDetails)
        {
            return staffDetails
                        .Select(x => new CostSubcategoryMonth
                        {
                            CostDetailStaffId = x.Id,
                            Id = x.CostDetailSubcategory.Id,
                            Name = x.CostDetailSubcategory.Name,
                            Description = x.Description,
                            Value = x.Value,
                            IdCategory = x.CostDetailSubcategory.CostDetailCategory.Id,
                            NameCategory = x.CostDetailSubcategory.CostDetailCategory.Name,
                            BudgetTypeId = x.BudgetTypeId
                        })
                        .OrderBy(x => x.Name)
                        .ToList();
        }
    }
}
