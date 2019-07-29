﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

namespace Sofco.Service.Implementations.ManagementReport
{
    public class ManagementReportStaffService : IManagementReportStaffService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ManagementReportStaffService> logger;
        private readonly IRoleManager roleManager;
        private readonly IUserData userData;
        private readonly AppSetting appSetting;
        private readonly IMailSender mailSender;

        public ManagementReportStaffService(IUnitOfWork unitOfWork,
            ILogMailer<ManagementReportStaffService> logger,
            IMailSender mailSender,
            IUserData userData,
            IOptions<AppSetting> appSettingOptions,
            IRoleManager roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            this.mailSender = mailSender;
            this.appSetting = appSettingOptions.Value;
            this.roleManager = roleManager;
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

            if (!CheckRoles(managementReport.Analytic))
            {
                response.AddError(Resources.Common.Forbidden);
                return response;
            }

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

                if (managementReport.Budgets.Any())
                {
                    response.Data.Budgets = managementReport.Budgets.Select(x => new BudgetItem(x)).ToList();

                    response.Data.BudgetTotal = response.Data.Budgets.Sum(x => x.Value);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }

            return response;
        }

        public Response<BudgetItem> AddBudget(int id, BudgetItem model)
        {
            var response = new Response<BudgetItem>();

            var managementReport = unitOfWork.ManagementReportRepository.GetStaffById(id);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            if (!CheckRoles(managementReport.Analytic))
            {
                response.AddError(Resources.Common.Forbidden);
                return response;
            }

            ValidateBudget(model, response);
            if (response.HasErrors()) return response;

            try
            {
                var budget = new Budget
                {
                    Value = model.Value,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    ManagementReportId = id
                };

                unitOfWork.ManagementReportRepository.AddBudget(budget);
                unitOfWork.Save();

                response.Data = new BudgetItem(budget);

                response.AddSuccess(Resources.ManagementReport.ManagementReport.BudgetAdded);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<BudgetItem> UpdateBudget(int id, BudgetItem model)
        {
            var response = new Response<BudgetItem>();

            var managementReport = unitOfWork.ManagementReportRepository.GetStaffById(id);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            if (!CheckRoles(managementReport.Analytic))
            {
                response.AddError(Resources.Common.Forbidden);
                return response;
            }

            var budget = unitOfWork.ManagementReportRepository.GetBudget(model.Id);

            if (budget == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.BudgetNotFound);
                return response;
            }

            ValidateBudget(model, response);
            if (response.HasErrors()) return response;

            try
            {
                budget.Value = model.Value;
                budget.Description = model.Description;

                unitOfWork.ManagementReportRepository.UpdateBudget(budget);
                unitOfWork.Save();

                response.Data = new BudgetItem(budget);

                response.AddSuccess(Resources.ManagementReport.ManagementReport.BudgetUpdated);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response DeleteBudget(int id, int budgetId)
        {
            var response = new Response();

            var managementReport = unitOfWork.ManagementReportRepository.GetStaffById(id);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            if (!CheckRoles(managementReport.Analytic))
            {
                response.AddError(Resources.Common.Forbidden);
                return response;
            }

            var budget = unitOfWork.ManagementReportRepository.GetBudget(budgetId);

            if (budget == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.BudgetNotFound);
                return response;
            }

            try
            {
                unitOfWork.ManagementReportRepository.DeleteBudget(budget);
                unitOfWork.Save();

                response.AddSuccess(Resources.ManagementReport.ManagementReport.BudgetDeleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<CostDetailStaffMonthModel> GetCostDetailMonth(int id, int month, int year)
        {
            var response = new Response<CostDetailStaffMonthModel> { Data = new CostDetailStaffMonthModel() };

            var managementReport = unitOfWork.ManagementReportRepository.Get(id);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            var monthYear = new DateTime(year, month, 1);

            CostDetail costDetail = unitOfWork.CostDetailRepository.GetByManagementReportAndMonthYear(id, monthYear);

            var allocations = unitOfWork.AllocationRepository.GetAllocationsByDate(monthYear);

            var listContracted = new List<ContractedModel>();
            var listOther = new List<CostMonthOther>();
            var resources = new List<CostMonthEmployeeStaff>();

            if (costDetail != null)
            {
                listContracted = this.Translate(costDetail.ContratedDetails.ToList());
                listOther = this.Translate(costDetail.CostDetailOthers.ToList());
                resources = this.Translate(costDetail.CostDetailResources.ToList(), monthYear, allocations, managementReport.AnalyticId);
                response.Data.Id = costDetail.Id;
                response.Data.TotalProvisioned = costDetail.TotalProvisioned;
            }

            response.Data.ManagementReportId = id;
            response.Data.MonthYear = monthYear;
            response.Data.Contracted = listContracted;
            response.Data.OtherResources = listOther;
            response.Data.Employees = resources;

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

                response.Data.MonthsHeader = new List<MonthDetailCostStaff>();
                response.Data.ManagementReportId = managementReport.Analytic.ManagementReport.Id;

                //Obtengo los meses que tiene la analitica
                var dates = SetDates(managementReport.Analytic);

                var costDetails = managementReport.CostDetails;

                for (DateTime date = new DateTime(dates.Item1.Year, dates.Item1.Month, 1).Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
                {
                    var monthHeader = new MonthDetailCostStaff();
                    monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                    monthHeader.MonthYear = date;

                    response.Data.MonthsHeader.Add(monthHeader);
                }

                response.Data.BudgetTypes = unitOfWork.ManagementReportRepository.GetTypesBudget().Select(x => new BudgetTypeItem(x)).ToList();
                response.Data.AllSubcategories = this.FillAllSubcategories();
                response.Data.CostCategories = this.FillCategoriesByMonth(response.Data.MonthsHeader, costDetails);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response UpdateCostDetailStaff(CostDetailStaffModel pDetailCost)
        {
            var response = new Response();
            try
            {
                var managementReport = unitOfWork.ManagementReportRepository.GetById(pDetailCost.ManagementReportId);
                var analytic = unitOfWork.AnalyticRepository.GetById(managementReport.AnalyticId);

                var listMonths = this.VerifyAnalyticMonths(managementReport, analytic.StartDateContract, analytic.EndDateContract);

                this.InsertUpdateCostDetailStaff(pDetailCost.CostCategories);

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

        private void InsertUpdateCostDetailStaff(List<CostCategory> costCategories)
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
                                                        .Union(month.SubcategoriesReal);

                        foreach (var subCatBudget in allSubcategories)
                        {
                            var entity = new CostDetailStaff();

                            if (subCatBudget.CostDetailStaffId > 0)
                            {
                                entity = unitOfWork.CostDetailStaffRepository.Get(subCatBudget.CostDetailStaffId);

                                if (subCatBudget.Value != entity.Value || subCatBudget.Description != entity.Description)
                                {
                                    entity.Value = subCatBudget.Value ?? 0;
                                    entity.Description = subCatBudget.Description;

                                    unitOfWork.CostDetailStaffRepository.Update(entity);
                                }
                            }
                            else
                            {
                                if (subCatBudget.Value > 0 || !string.IsNullOrEmpty(subCatBudget.Description))
                                {
                                    entity.Value = subCatBudget.Value ?? 0;
                                    entity.Description = subCatBudget.Description;
                                    entity.CostDetailId = month.CostDetailId;
                                    entity.CostDetailSubcategoryId = subCatBudget.Id;
                                    entity.BudgetTypeId = subCatBudget.BudgetTypeId;

                                    unitOfWork.CostDetailStaffRepository.Insert(entity);
                                }
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

        private void ValidateBudget(BudgetItem model, Response<BudgetItem> response)
        {
            if (model == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.BudgetModelNull);
            }

            if (model.Value <= 0)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.BudgetValueRequired);
            }

            if (string.IsNullOrWhiteSpace(model.Description))
            {
                response.AddError(Resources.ManagementReport.ManagementReport.BudgetDescriptionRequired);
            }

            if (model.StartDate == DateTime.MinValue)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.BudgetStartDateRequired);
            }
        }

        private bool CheckRoles(Analytic analytic)
        {
            var currentUser = userData.GetCurrentUser();

            if (roleManager.IsCdg() || (roleManager.IsDirector() && analytic.Sector.ResponsableUserId == currentUser.Id))
            {
                return true;
            }
            else if (roleManager.IsManager() && currentUser.Id == analytic.ManagerId.GetValueOrDefault())
            {
                return true;
            }

            return false;
        }

        private List<CostMonthOther> Translate(List<CostDetailOther> costDetailOthers)
        {
            return costDetailOthers
                .Where(t => t.CostDetailType.Name != EnumCostDetailType.AjusteGeneral.ToString())
                .Select(x => new CostMonthOther
                {
                    Id = x.Id,
                    Description = x.Description,
                    CostDetailId = x.CostDetailId,
                    TypeId = x.CostDetailTypeId,
                    TypeName = x.CostDetailType.Name,
                    Value = x.Value
                })
                .OrderBy(x => x.TypeId)
                .ToList();
        }

        private List<ContractedModel> Translate(List<ContratedDetail> contratedDetails)
        {
            return contratedDetails
                .Select(x => new ContractedModel
                {
                    CostDetailId = x.CostDetailId,
                    ContractedId = x.Id,
                    Name = x.Name,
                    Honorary = x.Honorary,
                    Insurance = x.Insurance,
                    Total = x.Honorary + x.Insurance
                })
                .OrderBy(x => x.Name)
                .ToList();
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

                var employee = unitOfWork.EmployeeRepository.GetWithSocialCharges(resource.EmployeeId);
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
                }
                else
                {
                    if (!decimal.TryParse(CryptographyHelper.Decrypt(resource.Value), out var salary)) salary = 0;
                    if (!decimal.TryParse(CryptographyHelper.Decrypt(resource.Charges), out var charges)) charges = 0;

                    item.Salary = salary;
                    item.Charges = charges;
                    item.Total = salary + charges;
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

                        var newValueCharges = (allocation.Percentage / 100) * charges;

                        item.Salary = salary * (allocation.Percentage / 100);
                        item.Charges = newValueCharges * (allocation.Percentage / 100);
                        item.Total = salary + newValueCharges;
                    }

                    list.Add(item);
                }
            }

            return list;
        }

        private List<CostCategory> FillCategoriesByMonth(IList<MonthDetailCostStaff> Months, ICollection<CostDetail> costDetails)
        {
            List<CostCategory> costCategories = new List<CostCategory>();

            var categories = unitOfWork.CostDetailRepository.GetCategories();

            foreach (var category in categories)
            {
                var detailCategory = new CostCategory();

                detailCategory.MonthsCategory = new List<MonthDetailCostStaff>();

                detailCategory.Id = category.Id;
                detailCategory.Name = category.Name;

                foreach (var mounth in Months)
                {
                    var monthDetail = new MonthDetailCostStaff();
                    monthDetail.SubcategoriesBudget = new List<CostSubcategory>();
                    monthDetail.SubcategoriesPfa1 = new List<CostSubcategory>();
                    monthDetail.SubcategoriesPfa2 = new List<CostSubcategory>();
                    monthDetail.SubcategoriesReal = new List<CostSubcategory>();

                    monthDetail.Display = mounth.Display;
                    monthDetail.MonthYear = mounth.MonthYear;

                    var costDetailMonth = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date).FirstOrDefault();
                    if (costDetailMonth != null)
                    {
                        monthDetail.CostDetailId = costDetailMonth.Id;

                        var subcategories = costDetailMonth.CostDetailStaff.Where(o => o.CostDetailSubcategory.CostDetailCategoryId == category.Id).ToList();
                        if (subcategories != null)
                        {
                            monthDetail.TotalBudget = subcategories.Where(x => x.BudgetType.Name.ToUpper() == "BUDGET").Sum(x => x.Value);
                            monthDetail.TotalPfa1 = subcategories.Where(x => x.BudgetType.Name.ToUpper() == "PFA1").Sum(x => x.Value);
                            monthDetail.TotalPfa2 = subcategories.Where(x => x.BudgetType.Name.ToUpper() == "PFA2").Sum(x => x.Value);
                            monthDetail.TotalReal = subcategories.Where(x => x.BudgetType.Name.ToUpper() == "REAL").Sum(x => x.Value);

                            foreach (var subcategory in subcategories)
                            {
                                var detailSubcategory = new CostSubcategory();

                                detailSubcategory.Id = subcategory.CostDetailSubcategoryId;
                                detailSubcategory.CostDetailStaffId = subcategory.Id;
                                detailSubcategory.Name = subcategory.CostDetailSubcategory.Name;
                                detailSubcategory.Description = subcategory.Description;
                                detailSubcategory.Value = subcategory.Value;
                                detailSubcategory.BudgetTypeId = subcategory.BudgetTypeId;

                                switch (subcategory.BudgetType.Name.ToUpper())
                                {
                                    case "BUDGET":
                                        monthDetail.SubcategoriesBudget.Add(detailSubcategory);
                                        break;
                                    case "PFA1":
                                        monthDetail.SubcategoriesPfa1.Add(detailSubcategory);
                                        break;
                                    case "PFA2":
                                        monthDetail.SubcategoriesPfa2.Add(detailSubcategory);
                                        break;
                                    case "REAL":
                                        monthDetail.SubcategoriesReal.Add(detailSubcategory);
                                        break;
                                }
                            }
                        }
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

        private void InsertTotalBudgets(List<CostCategory> costCategories)
        {
            decimal totalBudget = 0;
            decimal totalPFA1 = 0;
            decimal totalPFA2 = 0;
            decimal totalReal = 0;

            bool saveBudget = false;
            bool savePFA1 = false;
            bool savePFA2 = false;
            bool saveReal = false;

            try
            {
                foreach (var category in costCategories)
                {
                    foreach (var month in category.MonthsCategory)
                    {
                        totalBudget += month.SubcategoriesBudget.Sum(x => x.Value) ?? 0;
                        totalPFA1 += month.SubcategoriesPfa1.Sum(x => x.Value) ?? 0;
                        totalPFA2 += month.SubcategoriesPfa2.Sum(x => x.Value) ?? 0;
                        totalReal += month.SubcategoriesReal.Sum(x => x.Value) ?? 0;

                        if(month.SubcategoriesBudget.Count > 0)
                            saveBudget = true;
                        if (month.SubcategoriesPfa1.Count > 0)
                            savePFA1 = true;
                        if (month.SubcategoriesPfa2.Count > 0)
                            savePFA2 = true;
                        if (month.SubcategoriesReal.Count > 0)
                            saveReal = true;
                    }
                }

                //if (saveBudget)
                //{
                //    var budget = new Budget
                //    {
                //        Value = model.Value,
                //        Description = model.Description,
                //        StartDate = model.StartDate,
                //        ManagementReportId = id
                //    };

                //    unitOfWork.ManagementReportRepository.AddBudget(budget);
                //}
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                throw ex;
            }
        }



    }
}
