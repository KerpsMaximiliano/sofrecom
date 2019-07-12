using System;
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

            var employees = unitOfWork.AllocationRepository.GetAllocationsByDate(monthYear);

            var listContracted = new List<ContractedModel>();
            var listOther = new List<CostMonthOther>();
            var resources = new List<CostMonthEmployeeStaff>();

            if (costDetail != null)
            {
                listContracted = this.Translate(costDetail.ContratedDetails.ToList());
                listOther = this.Translate(costDetail.CostDetailOthers.ToList());
                resources = this.Translate(costDetail.CostDetailResources.ToList(), monthYear, employees, managementReport.AnalyticId);
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

                var employee = unitOfWork.EmployeeRepository.Get(resource.EmployeeId);
                item.Name = employee?.Name;
                item.UserId = resource.UserId;
                item.MonthYear = monthYear;

                if (!decimal.TryParse(CryptographyHelper.Decrypt(resource.Value), out var salary)) salary = 0;
                if (!decimal.TryParse(CryptographyHelper.Decrypt(resource.Charges), out var charges)) charges = 0;

                item.Salary = salary;
                item.Charges = charges;
                item.Total = salary + charges;

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
                        if (!decimal.TryParse(CryptographyHelper.Decrypt(allocation.Employee.Salary), out var salary)) salary = 0;
                        if (!decimal.TryParse(CryptographyHelper.Decrypt(allocation.Employee.PrepaidAmount), out var charges)) charges = 0;

                        item.Salary = salary * (allocation.Percentage / 100);
                        item.Charges = charges * (allocation.Percentage / 100);
                        item.Total = salary + charges;
                    }

                    list.Add(item);
                }
            }

            return list;
        }
    }
}
