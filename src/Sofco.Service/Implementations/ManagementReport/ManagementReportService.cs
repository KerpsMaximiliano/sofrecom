using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models.Common;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Crm;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using Sofco.Framework.MailData;
using Sofco.Resources.Mails;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly AppSetting appSetting;
        private readonly EmailConfig emailConfig;
        private readonly IManagementReportFileManager managementReportFileManager;
        private readonly IMailSender mailSender;

        public ManagementReportService(IUnitOfWork unitOfWork,
            ILogMailer<ManagementReportService> logger,
            IMailSender mailSender,
            IUserData userData,
            IProjectData projectData,
            IOptions<AppSetting> appSettingOptions,
            IOptions<EmailConfig> emailConfigOptions,
            ISolfacService solfacService,
            IRoleManager roleManager,
            IManagementReportFileManager managementReportFileManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.userData = userData;
            this.mailSender = mailSender;
            this.appSetting = appSettingOptions.Value;
            this.projectData = projectData;
            this.roleManager = roleManager;
            this.solfacService = solfacService;
            this.managementReportFileManager = managementReportFileManager;
            this.emailConfig = emailConfigOptions.Value;
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
                response.Data.ManagerId = analytic?.ManagerId;
                response.Data.ManagementReportId = analytic.ManagementReport.Id;
                response.Data.ManamementReportStartDate = analytic.ManagementReport.StartDate;
                response.Data.ManamementReportEndDate = analytic.ManagementReport.EndDate;
                response.Data.AnalyticStatus = analytic.Status;
                response.Data.Status = analytic.ManagementReport.Status;
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
                //var diccionary = new Dictionary<string, decimal>();

                response.Data.PurchaseOrders = purchaseOrders.Where(x => x.Status == PurchaseOrderStatus.Valid).Select(x => x.Title).ToList();
                //response.Data.Ammounts = new List<AmmountItem>();

                //foreach (var purchaseOrder in purchaseOrders)
                //{
                //    foreach (var detail in purchaseOrder.AmmountDetails)
                //    {
                //        if (diccionary.ContainsKey(detail.Currency.Text))
                //        {
                //            diccionary[detail.Currency.Text] += detail.Ammount;
                //        }
                //        else
                //        {
                //            diccionary.Add(detail.Currency.Text, detail.Ammount);
                //        }
                //    }
                //}

                //foreach (var key in diccionary.Keys)
                //{
                //    response.Data.Ammounts.Add(new AmmountItem { Currency = key, Value = diccionary[key] });
                //}
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

            var dates = SetDates(analytic);

            response.Data.MonthsHeader = new List<MonthBillingHeaderItem>();
            response.Data.Projects = new List<ProjectOption>();

            response.Data.ManagerId = analytic.Manager.ExternalManagerId;

            var costDetails = unitOfWork.CostDetailRepository.GetByManagementReportAndDates(analytic.ManagementReport.Id, dates.Item1.Date, dates.Item2.Date);
            var billings = unitOfWork.ManagementReportBillingRepository.GetByManagementReportAndDates(analytic.ManagementReport.Id, dates.Item1.Date, dates.Item2.Date);
            var currencyExchanges = unitOfWork.CurrencyExchangeRepository.Get(dates.Item1.Date, dates.Item2.Date);

            for (DateTime date = dates.Item1.Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
            {
                var monthHeader = new MonthBillingHeaderItem();
                monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                monthHeader.Year = date.Year;
                monthHeader.Month = date.Month;
                monthHeader.MonthYear = date;

                var costDetailMonth = costDetails.SingleOrDefault(x => x.MonthYear.Date == date.Date);

                var billingMonth = billings.SingleOrDefault(x => x.MonthYear.Date == date.Date);

                var currencyExchange = currencyExchanges.Where(x => x.Date.Month == date.Month && x.Date.Year == date.Year);

                monthHeader.Exchanges = currencyExchange.Select(x => new CurrencyExchangeItemModel { CurrencyDesc = x.Currency.Text, Exchange = x.Exchange }).ToList();

                if (billingMonth != null)
                {
                    monthHeader.ValueEvalProp = billingMonth.EvalPropBillingValue;
                    monthHeader.BillingMonthId = billingMonth.Id;
                    monthHeader.Closed = billingMonth.Closed;
                    monthHeader.EvalPropDifference = billingMonth.EvalPropDifference;
                    monthHeader.Comments = billingMonth.Comments;

                    if (billingMonth.BilledResources > 0)
                        monthHeader.ResourceQuantity = billingMonth.BilledResources;
                    else
                    {
                        if (costDetailMonth != null)
                            monthHeader.ResourceQuantity = costDetailMonth.CostDetailProfiles.Count + costDetailMonth.CostDetailResources.Count;
                    }
                }
                else
                {
                    if (costDetailMonth != null)
                        monthHeader.ResourceQuantity = costDetailMonth.CostDetailProfiles.Count + costDetailMonth.CostDetailResources.Count;
                }

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

                        var currencyExchange = currencyExchanges.SingleOrDefault(x => x.Currency.CrmId == hito.MoneyId && x.Date.Month == hito.StartDate.Month && x.Date.Year == hito.StartDate.Year);

                        var montoOriginalPesos = hito.AmountOriginal;

                        if (currencyExchange != null && currencyExchange?.CurrencyId != appSetting.CurrencyPesos)
                        {
                            montoOriginalPesos *= currencyExchange.Exchange;
                        }

                        var billingRowItem = new BillingHitoItem
                        {
                            Description = hito.Name,
                            Id = hito.Id,
                            ProjectId = project.CrmId,
                            ProjectName = $"{project.OpportunityNumber} {project.Name}",
                            CurrencyId = hito.MoneyId,
                            CurrencyName = hito.Money,
                            Month = hito.Month,
                            OpportunityNumber = project.OpportunityNumber,
                            Date = hito.StartDate,
                            MonthValues = new List<MonthBiilingRowItem>()
                        };

                        var rowItem = new MonthBiilingRowItem
                        {
                            Month = hito.StartDate.Month,
                            Year = hito.StartDate.Year,
                            MonthYear = new DateTime(hito.StartDate.Year, hito.StartDate.Month, 1),
                            Value = hito.Ammount,
                            OriginalValue = hito.AmountOriginal,
                            OriginalValuePesos = montoOriginalPesos,
                            Status = hito.Status
                        };

                        var montoPesos = hito.Ammount;

                        if (currencyExchange != null && currencyExchange.CurrencyId != appSetting.CurrencyPesos)
                        {
                            montoPesos *= currencyExchange.Exchange;
                        }

                        if (existHito != null)
                        {
                            rowItem.SolfacId = existHito.SolfacId;

                            if (existHito.Solfac.CurrencyExchange.HasValue && existHito.Solfac.CurrencyExchange > 0)
                            {
                                rowItem.ValuePesos = hito.Ammount * existHito.Solfac.CurrencyExchange.Value;
                            }
                            else
                            {
                                rowItem.ValuePesos = montoPesos;
                            }
                        }
                        else
                        {
                            rowItem.ValuePesos = montoPesos;
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

                    foreach (var monthValue in billingTotal.MonthValues)
                    {
                        var currencyExchange = currencyExchanges.SingleOrDefault(x => x.Currency.CrmId == billingTotal.CurrencyId && x.Date.Month == monthValue.Month && x.Date.Year == monthValue.Year);

                        var header = response.Data.MonthsHeader.SingleOrDefault(x => x.Month == monthValue.Month && x.Year == monthValue.Year);

                        if (header != null)
                        {
                            if (currencyExchange != null)
                            {
                                header.TotalBilling += (monthValue.Value * currencyExchange.Exchange);
                            }
                            else
                            {
                                header.TotalBilling += monthValue.Value;
                            }
                        }
                    }

                }
            }

            return response;
        }

        public Response<CostResourceEmployee> GetCostDetailByEmployee(string serviceId, int employeeId)
        {
            var response = new Response<CostResourceEmployee> { Data = new CostResourceEmployee() };
            var monthsHeader = new List<MonthHeaderCost>();

            try
            {
                var analytic = unitOfWork.AnalyticRepository.GetByServiceWithManagementReport(serviceId);

                if (analytic == null)
                {
                    response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                    return response;
                }

                var dates = SetDates(analytic);

                var managementReport = unitOfWork.ManagementReportRepository.GetById(analytic.ManagementReport.Id);
                var costDetails = managementReport.CostDetails;

                for (DateTime date = new DateTime(dates.Item1.Year, dates.Item1.Month, 1).Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
                {
                    var monthHeader = new MonthHeaderCost();
                    monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                    monthHeader.MonthYear = date;
                    monthHeader.Month = date.Month;
                    monthHeader.Year = date.Year;

                    var costDetailMonth = costDetails.SingleOrDefault(x => x.MonthYear.Date == date.Date);
                    if (costDetailMonth != null)
                    {
                        monthHeader.HasReal = costDetailMonth.HasReal;
                        monthHeader.TotalContracted = costDetailMonth.ContratedDetails.Sum(x => x.Honorary) + costDetailMonth.ContratedDetails.Sum(x => x.Insurance);
                        monthHeader.TotalBilling = costDetailMonth.TotalBilling;
                    }

                    monthsHeader.Add(monthHeader);
                }


                response.Data = FillCostEmployeeByMonth(analytic.Id, employeeId, monthsHeader, costDetails, dates.Item1, dates.Item2);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response<CostDetailModel> GetCostDetail(string serviceId)
        {
            var response = new Response<CostDetailModel> { Data = new CostDetailModel() };
            try
            {
                var analytic = unitOfWork.AnalyticRepository.GetByServiceWithManagementReport(serviceId);

                if (analytic == null)
                {
                    response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                    return response;
                }

                response.Data.AnalyticId = analytic.Id;
                //Obtengo los meses que tiene la analitica
                response.Data.MonthsHeader = new List<MonthHeaderCost>();

                response.Data.ManagerId = analytic.Manager.ExternalManagerId;
                response.Data.ManagementReportId = analytic.ManagementReport.Id;

                var dates = SetDates(analytic);

                var managementReport = unitOfWork.ManagementReportRepository.GetById(analytic.ManagementReport.Id);
                var costDetails = managementReport.CostDetails;
                var billings = unitOfWork.ManagementReportBillingRepository.GetByManagementReportAndDates(analytic.ManagementReport.Id, dates.Item1.Date, dates.Item2.Date);
                var currencyExchanges = unitOfWork.CurrencyExchangeRepository.Get(dates.Item1.Date, dates.Item2.Date);

                for (DateTime date = new DateTime(dates.Item1.Year, dates.Item1.Month, 1).Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
                {
                    var monthHeader = new MonthHeaderCost();
                    monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                    monthHeader.MonthYear = date;
                    monthHeader.Month = date.Month;
                    monthHeader.Year = date.Year;

                    var currencyExchange = currencyExchanges.Where(x => x.Date.Month == date.Month && x.Date.Year == date.Year);
                    monthHeader.CurrencyMonth = currencyExchange.Select(x => new CurrencyExchangeItemModel { CurrencyDesc = x.Currency.Text, Exchange = x.Exchange, CurrencyId = x.CurrencyId }).ToList();

                    var costDetailMonth = costDetails.SingleOrDefault(x => x.MonthYear.Date == date.Date);
                    if (costDetailMonth != null)
                    {
                        monthHeader.HasReal = costDetailMonth.HasReal;
                        monthHeader.TotalContracted = costDetailMonth.ContratedDetails.Sum(x => x.Honorary) + costDetailMonth.ContratedDetails.Sum(x => x.Insurance);
                        monthHeader.TotalBilling = costDetailMonth.TotalBilling;
                    }

                    var billingMonth = billings.SingleOrDefault(x => x.MonthYear.Date == date.Date);
                    if (billingMonth != null)
                    {
                        monthHeader.ValueEvalProp = billingMonth.EvalPropExpenseValue;
                        //monthHeader.ValueMarginEvalProp = billingMonth.EvalPropMarginValue;

                        monthHeader.BillingMonthId = billingMonth.Id;

                        if (costDetailMonth != null)
                        {
                            monthHeader.Closed = costDetailMonth.Closed;
                            monthHeader.CostDetailId = costDetailMonth.Id;
                        }
                    }

                    response.Data.MonthsHeader.Add(monthHeader);
                }

                //Obtengo las categorias
                List<CostDetailCategories> Allcategories = unitOfWork.CostDetailRepository.GetCategories();

                //Categorias exceptuadas
                List<string> catExcep = new List<string>(new string[] { EnumCostDetailType.Red, EnumCostDetailType.Infraestructura, EnumCostDetailType.InformeFinal });
                var categories = Allcategories.Where(x => !catExcep.Any(y => x.Name == y)).ToList();

                //Mapeo Los demas datos
                var AllCostResources = FillFundedResoursesByMonth(response.Data.MonthsHeader, costDetails, categories);

                response.Data.CostEmployees = FillCostEmployeesByMonth(analytic.Id, response.Data.MonthsHeader, costDetails, dates.Item1, dates.Item2);
                response.Data.FundedResourcesEmployees = AllCostResources.Where(r => r.BelongEmployee == true).ToList();
                response.Data.FundedResources = AllCostResources.Where(r => r.show == true && r.BelongEmployee == false).ToList();
                response.Data.OtherResources = AllCostResources.Where(r => r.show == false && r.BelongEmployee == false).OrderBy(r => r.Display).ToList();
                response.Data.CostProfiles = FillProfilesByMonth(response.Data.MonthsHeader, costDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response<CostDetailMonthModel> GetCostDetailMonth(string pServiceId, int pMonth, int pYear)
        {
            var response = new Response<CostDetailMonthModel> { Data = new CostDetailMonthModel() };
            try
            {
                var analytic = unitOfWork.AnalyticRepository.GetByServiceWithManagementReport(pServiceId);

                if (analytic == null)
                {
                    response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                    return response;
                }

                var monthYear = new DateTime(pYear, pMonth, 1);

                CostDetail costDetail = unitOfWork.CostDetailRepository.GetByManagementReportAndMonthYear(analytic.ManagementReport.Id, monthYear);
                bool getReal = false;
                if (costDetail.HasReal)
                {
                    getReal = true;
                }

                List<ContractedModel> listContracted = this.Translate(costDetail.ContratedDetails.ToList());
                List<CostMonthOther> listOther = this.Translate(costDetail.CostDetailOthers.Where(x => x.IsReal == getReal).ToList());

                if (costDetail.CostDetailProfiles.Sum(x => x.Value) > 0)
                {
                    response.Data.HasCostProfile = true;
                }

                if (!getReal)
                {
                    foreach (var other in listOther)
                    {
                        other.Id = 0;
                    }
                }

                response.Data.Id = costDetail.Id;

                response.Data.Provision = costDetail.Provision;
                response.Data.TotalBilling = costDetail.TotalBilling;
                response.Data.TotalProvisioned = costDetail.TotalProvisioned;

                response.Data.ManagementReportId = analytic.ManagementReport.Id;
                response.Data.MonthYear = costDetail.MonthYear;
                response.Data.Contracted = listContracted;
                response.Data.OtherResources = listOther.OrderBy(x => x.CategoryName).ThenBy(x => x.SubcategoryName).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response<MonthOther> GetOtherTypeAndCostDetail(int idCategory, int idCostDetail)
        {
            var response = new Response<MonthOther> { Data = new MonthOther() };

            try
            {
                var listOther = unitOfWork.CostDetailOtherRepository.GetByTypeAndCostDetail(idCategory, idCostDetail);
                var listBudget = listOther.Where(x => x.IsReal == false).ToList();
                var subcategories = unitOfWork.CostDetailRepository.GetSubcategories();

                response.Data.CostMonthOther = this.Translate(listBudget);
                response.Data.Subcategories = this.Translate(subcategories.Where(x => x.CostDetailCategoryId == idCategory).ToList());
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
                var managementReport = unitOfWork.ManagementReportRepository.GetById(pDetailCost.ManagementReportId);
                var analytic = unitOfWork.AnalyticRepository.GetById(managementReport.AnalyticId);

                var listMonths = this.VerifyAnalyticMonths(managementReport, analytic.StartDateContract, analytic.EndDateContract);

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
                        EmployeesWithAllMonths = this.AddAnalyticMonthsToEmployees(pDetailCost.CostEmployees, managementReport.Id, analytic.StartDateContract, analytic.EndDateContract);
                    }

                    this.InsertUpdateCostDetailResources(EmployeesWithAllMonths, managementReport.CostDetails.ToList(), managementReport.Id);
                }
                if (pDetailCost.FundedResources.Count > 0)
                    this.InsertUpdateCostDetailOther(pDetailCost.FundedResources, managementReport.CostDetails.ToList());
                if (pDetailCost.CostProfiles.Count > 0)
                    this.InsertUpdateCostDetailProfile(pDetailCost.CostProfiles, managementReport.CostDetails.ToList());

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
            _detailModel.ManagementReportId = pMonthDetail.ManagementReportId;
            _detailModel.CostEmployees = new List<CostResourceEmployee>();
            _detailModel.FundedResources = new List<CostResource>();
            decimal totalSalary = 0;

            var budgetTypes = unitOfWork.ManagementReportRepository.GetTypesBudget();

            try
            {
                foreach (var employee in pMonthDetail.Employees)
                {
                    var cost = new CostResourceEmployee();
                    cost.MonthsCost = new List<MonthDetailCost>();
                    MonthDetailCost month = new MonthDetailCost();

                    cost.EmployeeId = employee.EmployeeId;
                    cost.UserId = employee.UserId;
                    month.MonthYear = pMonthDetail.MonthYear;

                    if (pMonthDetail.IsReal)
                    {
                        month.Real.Id = employee.Id;
                        month.Real.Value = employee.Salary;
                        month.Real.Charges = employee.Charges;
                        month.Real.BudgetTypeId = budgetTypes.Where(x => x.Name == EnumBudgetType.Real).FirstOrDefault().Id;
                    }
                    else
                    {
                        month.Budget.Id = employee.Id;
                        month.Budget.Value = employee.Salary;
                        month.Budget.Charges = employee.Charges;
                    }

                    totalSalary += employee.Salary ?? 0;
                    totalSalary += employee.Charges ?? 0;

                    cost.MonthsCost.Add(month);
                    _detailModel.CostEmployees.Add(cost);
                }

                foreach (var otherRes in pMonthDetail.OtherResources)
                {
                    CostResource cost = new CostResource();
                    cost.MonthsCost = new List<MonthDetailCost>();
                    MonthDetailCost month = new MonthDetailCost();

                    cost.TypeId = otherRes.SubcategoryId;
                    cost.CurrencyId = otherRes.CurrencyId == 0 ? appSetting.CurrencyPesos : otherRes.CurrencyId;
                    month.MonthYear = pMonthDetail.MonthYear;
                    cost.TypeName = otherRes.CategoryName;

                    if (pMonthDetail.IsReal)
                    {
                        month.Real.Id = otherRes.Id;
                        month.Real.Value = otherRes.Value;
                        month.Real.Description = otherRes.Description;
                    }
                    else
                    {
                        month.Budget.Id = otherRes.Id;
                        month.Budget.Value = otherRes.Value;
                        month.Budget.Description = otherRes.Description;
                    }

                    cost.MonthsCost.Add(month);
                    _detailModel.FundedResources.Add(cost);
                }

                var costDetails = unitOfWork.CostDetailRepository.GetByManagementReport(pMonthDetail.ManagementReportId);

                this.InsertUpdateCostDetailResources(_detailModel.CostEmployees, costDetails, pMonthDetail.ManagementReportId, pMonthDetail.IsReal);
                this.InsertUpdateCostDetailOther(_detailModel.FundedResources, costDetails, pMonthDetail.IsReal);
                this.UpdateContracted(pMonthDetail.Contracted, costDetails, pMonthDetail.MonthYear);

                var costDetailMonth = costDetails.SingleOrDefault(x => x.MonthYear.Date == pMonthDetail.MonthYear.Date);

                if (costDetailMonth != null)
                {
                    costDetailMonth.Provision = pMonthDetail.Provision ?? pMonthDetail.Provision.GetValueOrDefault();
                    costDetailMonth.TotalBilling = pMonthDetail.TotalBilling ?? pMonthDetail.TotalBilling.GetValueOrDefault();
                    costDetailMonth.TotalProvisioned = pMonthDetail.TotalProvisioned ?? pMonthDetail.TotalProvisioned.GetValueOrDefault();

                    unitOfWork.CostDetailRepository.UpdateTotals(costDetailMonth);
                }

                if (costDetailMonth.HasReal == false && pMonthDetail.IsReal == true)
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

        public Response UpdateContracted(List<ContractedModel> pContracted, IList<CostDetail> costDetails, DateTime monthYear)
        {
            var response = new Response();
            try
            {
                foreach (var hired in pContracted)
                {
                    ContratedDetail entity = new ContratedDetail();

                    if (hired.ContractedId > 0)
                    {
                        entity = unitOfWork.ContratedDetailRepository.Get(hired.ContractedId);

                        if (entity.Honorary != hired.Honorary || entity.Insurance != hired.Insurance || entity.Name != hired.Name)
                        {
                            entity.Honorary = hired.Honorary ?? 0;
                            entity.Insurance = hired.Insurance ?? 0;
                            entity.Name = hired.Name;
                        }

                        unitOfWork.ContratedDetailRepository.Update(entity);
                    }
                    else
                    {
                        entity.CostDetailId = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == monthYear.Date).FirstOrDefault().Id;
                        entity.Name = hired.Name;
                        entity.Insurance = hired.Insurance ?? 0;
                        entity.Honorary = hired.Honorary ?? 0;

                        unitOfWork.ContratedDetailRepository.Insert(entity);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
                throw ex;
            }

            return response;
        }

        public Response DeleteContracted(int ContractedId)
        {
            var response = new Response();
            try
            {
                var entity = unitOfWork.ContratedDetailRepository.Get(ContractedId);

                if (entity != null)
                {
                    unitOfWork.ContratedDetailRepository.Delete(entity);
                    unitOfWork.Save();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }
            return response;
        }

        public Response DeleteOtherResource(int id)
        {
            var response = new Response();
            try
            {
                var entity = unitOfWork.CostDetailOtherRepository.Get(id);

                if (entity != null)
                {
                    unitOfWork.CostDetailOtherRepository.Delete(entity);
                    unitOfWork.Save();
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }
            return response;
        }

        public Response<byte[]> CreateTracingReport(TracingModel tracing)
        {
            var response = new Response<byte[]>();

            try
            {
                var excel = managementReportFileManager.CreateTracingExcel(tracing);

                response.Data = excel.GetAsByteArray();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ExportFileError);
            }

            return response;
        }

        public Response Send(ManagementReportSendModel model)
        {
            var response = new Response();

            var report = unitOfWork.ManagementReportRepository.GetWithAnalytic(model.Id);

            if (report == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            if (!model.Status.HasValue)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.StatusRequired);
                return response;
            }

            if (report.Status == model.Status)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.CannotChangeStatus);
                return response;
            }

            if (report.Status == ManagementReportStatus.Closed)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.IsClosed);
            }
            else
            {
                if (report.Status == ManagementReportStatus.CdgPending && model.Status == ManagementReportStatus.ManagerPending && !roleManager.IsCdg())
                {
                    response.AddError(Resources.ManagementReport.ManagementReport.CannotChangeStatus);
                }

                if (report.Status == ManagementReportStatus.ManagerPending && model.Status == ManagementReportStatus.CdgPending && !roleManager.IsManager())
                {
                    response.AddError(Resources.ManagementReport.ManagementReport.CannotChangeStatus);
                }
            }

            if (response.HasErrors()) return response;

            try
            {
                report.Status = model.Status.Value;
                unitOfWork.ManagementReportRepository.UpdateStatus(report);
                unitOfWork.Save();

                response.AddSuccess(Resources.ManagementReport.ManagementReport.SendSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            SendMail(model, report, response);

            return response;
        }

        public Response Close(ManagementReportCloseModel model)
        {
            var response = new Response();

            var billing = unitOfWork.ManagementReportBillingRepository.Get(model.BillingId);

            if (billing == null) response.AddError(Resources.ManagementReport.ManagementReportBilling.NotFound);

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
                billing.Closed = true;
                detailCost.Closed = true;

                unitOfWork.ManagementReportBillingRepository.Close(billing);
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
                if (unitOfWork.ManagementReportRepository.AllMonthsAreClosed(billing.ManagementReportId))
                {
                    var managementReport = new Domain.Models.ManagementReport.ManagementReport
                    {
                        Id = billing.ManagementReportId,
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

        private void SendMail(ManagementReportSendModel model, Domain.Models.ManagementReport.ManagementReport report, Response response)
        {
            try
            {
                var subject = string.Format(MailSubjectResource.ManagementReport, report.Analytic.Title,
                    report.Analytic.AccountName, report.Analytic.ServiceName);

                var body = model.Status == ManagementReportStatus.CdgPending
                    ? MailMessageResource.ManagementReportManager
                    : MailMessageResource.ManagementReportCdg;

                var cdgGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.CdgCode);
                var recipientsList = new List<string> { report.Analytic.Manager.Email, cdgGroup.Email };

                var data = new MailDefaultData()
                {
                    Title = subject,
                    Message = body,
                    Recipients = recipientsList.Distinct().ToList()
                };

                mailSender.Send(data);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddWarning(Resources.Common.ErrorSendMail);
            }
        }

        public Response UpdateDates(int id, ManagementReportUpdateDates model)
        {
            var response = new Response();

            var managementReport = unitOfWork.ManagementReportRepository.GetWithCostDetailsAndBillings(id);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            if (!model.StartDate.HasValue) response.AddError(Resources.ManagementReport.ManagementReport.StartDateRequired);
            if (!model.EndDate.HasValue) response.AddError(Resources.ManagementReport.ManagementReport.EndDateRequired);

            if (response.HasErrors()) return response;

            try
            {
                managementReport.StartDate = model.StartDate.GetValueOrDefault().Date;
                managementReport.EndDate = model.EndDate.GetValueOrDefault().Date;

                for (var date = new DateTime(managementReport.StartDate.Year, managementReport.StartDate.Month, 1).Date;
                    date.Date <= managementReport.EndDate.Date;
                    date = date.AddMonths(1))
                {
                    if (managementReport.CostDetails != null)
                    {
                        if (managementReport.CostDetails.All(x => x.MonthYear.Date != date.Date))
                        {
                            managementReport.CostDetails.Add(new CostDetail
                            {
                                ManagementReportId = managementReport.Id,
                                MonthYear = date.Date
                            });
                        }
                    }
                    else
                    {
                        managementReport.CostDetails = new List<CostDetail>();
                        managementReport.CostDetails.Add(new CostDetail
                        {
                            ManagementReportId = managementReport.Id,
                            MonthYear = date.Date
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(managementReport.Analytic.AccountId))
                    {
                        if (managementReport.Billings != null)
                        {
                            if (managementReport.Billings.All(x => x.MonthYear.Date != date.Date))
                            {
                                managementReport.Billings.Add(new ManagementReportBilling
                                {
                                    ManagementReportId = managementReport.Id,
                                    MonthYear = date.Date
                                });
                            }
                        }
                        else
                        {
                            managementReport.Billings = new List<ManagementReportBilling>();
                            managementReport.Billings.Add(new ManagementReportBilling
                            {
                                ManagementReportId = managementReport.Id,
                                MonthYear = date.Date
                            });
                        }
                    }
                }

                unitOfWork.ManagementReportRepository.Update(managementReport);
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
                        biilingRowItem = new MonthBiilingRowItem { Year = date.Year, Month = date.Month, Value = hito.Ammount, MonthYear = new DateTime(date.Year, date.Month, 1) };
                    }
                    else
                    {
                        biilingRowItem = new MonthBiilingRowItem { Year = date.Year, Month = date.Month, Value = 0, MonthYear = new DateTime(date.Year, date.Month, 1) };
                    }

                    totalBilling.MonthValues.Add(biilingRowItem);
                }

                response.Data.Totals.Add(totalBilling);
            }
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

        private bool CheckRoles(Analytic analytic)
        {
            var currentUser = userData.GetCurrentUser();

            if (roleManager.IsCdg())
            {
                return true;
            }

            if (roleManager.IsDirector() && currentUser.Id == analytic.Sector.ResponsableUserId)
            {
                return true;
            }

            if (roleManager.IsManager() && currentUser.Id == analytic.ManagerId.GetValueOrDefault())
            {
                return true;
            }

            return false;
        }

        private CostResourceEmployee FillCostEmployeeByMonth(int IdAnalytic, int employeeId, IList<MonthHeaderCost> Months, ICollection<CostDetail> costDetails, DateTime startDate, DateTime endDate)
        {
            var budgetTypes = unitOfWork.ManagementReportRepository.GetTypesBudget();

            var employee = unitOfWork.EmployeeRepository.GetWithSocialCharges(employeeId);

            var costResourceEmployee = FillEmployee(IdAnalytic, Months, costDetails, employee, budgetTypes);

            return costResourceEmployee;
        }

        public List<CostResourceEmployee> FillCostEmployeesByMonth(int IdAnalytic, IList<MonthHeaderCost> Months, ICollection<CostDetail> costDetails, DateTime startDate, DateTime endDate)
        {
            List<CostResourceEmployee> costEmployees = new List<CostResourceEmployee>();
            var budgetTypes = unitOfWork.ManagementReportRepository.GetTypesBudget();

            //Obtengo los empleados de la analitica
            var EmployeesAnalytic = unitOfWork.EmployeeRepository.GetByAnalyticWithSocialCharges(IdAnalytic, startDate, endDate);

            // Obtengo los empleados del reporte que no estan en la analitica.
            var IdEmployeesWithOutAnalytic = costDetails
                                                .Where(x => x.MonthYear.Date >= startDate.Date && x.MonthYear.Date <= endDate.Date)
                                                .SelectMany(x => x.CostDetailResources.Select(d => d.EmployeeId))
                                                .Distinct()
                                                .Except(EmployeesAnalytic.Select(x => x.Id))
                                                .ToArray();

            var employeesWithOutAnalytic = unitOfWork.EmployeeRepository.GetById(IdEmployeesWithOutAnalytic);

            var allEmployees = EmployeesAnalytic.Union(employeesWithOutAnalytic).ToList();

            foreach (var employee in allEmployees)
            {
                var detailEmployee = FillEmployee(IdAnalytic, Months, costDetails, employee, budgetTypes);

                costEmployees.Add(detailEmployee);

                //if (employeeHasAllocation)
                //{
                //    costEmployees.Add(detailEmployee);
                //}
            }

            return costEmployees.OrderBy(e => e.Display).ToList();
        }

        private CostResourceEmployee FillEmployee(int IdAnalytic, IList<MonthHeaderCost> Months, ICollection<CostDetail> costDetails, Employee employee,
            List<BudgetType> budgetTypes)
        {
            var employeeHasAllocation = false;
            var user = unitOfWork.UserRepository.GetByEmail(employee.Email);

            var detailEmployee = new CostResourceEmployee();
            detailEmployee.MonthsCost = new List<MonthDetailCost>();

            detailEmployee.EmployeeId = employee.Id;
            detailEmployee.UserId = user?.Id;
            detailEmployee.Display = employee.Name + " - " + employee.EmployeeNumber;
            detailEmployee.TypeName = EnumCostDetailType.Empleados;

            var canViewSensibleData = roleManager.CanViewSensibleData();

            foreach (var mounth in Months)
            {
                var monthDetail = new MonthDetailCost();

                var costDetailMonth = costDetails.FirstOrDefault(c =>
                    new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date);

                if (costDetailMonth != null)
                {
                    foreach (var typeBudget in budgetTypes)
                    {
                        var auxTypeCost = new Cost();
                        auxTypeCost.Adjustment = 0;
                        auxTypeCost.BudgetTypeId = typeBudget.Id;

                        var monthValue = costDetailMonth.CostDetailResources.FirstOrDefault(e =>
                            e.EmployeeId == employee.Id && e.BudgetTypeId == typeBudget.Id);

                        if (monthValue != null)
                        {
                            if (!string.IsNullOrWhiteSpace(monthValue.Value))
                            {
                                if (canViewSensibleData)
                                {
                                    if (!decimal.TryParse(CryptographyHelper.Decrypt(monthValue.Value), out var salary))
                                        salary = 0;
                                    if (!decimal.TryParse(CryptographyHelper.Decrypt(monthValue.Charges), out var charges))
                                        charges = 0;

                                    auxTypeCost.Value = salary;
                                    auxTypeCost.OriginalValue = salary;
                                    auxTypeCost.Charges = charges;
                                    auxTypeCost.Adjustment = monthValue.Adjustment ?? 0;

                                    monthDetail.CanViewSensibleData = true;

                                    if (salary > 0)
                                    {
                                        monthDetail.ChargesPercentage = (charges / salary) * 100;
                                    }
                                }

                                auxTypeCost.Id = monthValue.Id;
                            }
                            else
                            {
                                if (canViewSensibleData)
                                {
                                    monthDetail.CanViewSensibleData = true;

                                    if (employee.SocialCharges != null && employee.SocialCharges.Any())
                                    {
                                        var socialCharge = employee?.SocialCharges.FirstOrDefault(x =>
                                            x.Year == mounth.MonthYear.Year && x.Month == mounth.MonthYear.Month);

                                        if (socialCharge != null)
                                        {
                                            var allocation = employee.Allocations.FirstOrDefault(x =>
                                                x.AnalyticId == IdAnalytic && x.StartDate.Date == mounth.MonthYear.Date);

                                            if (allocation != null)
                                            {
                                                if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.SalaryTotal),
                                                    out var salary)) salary = 0;
                                                if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.ChargesTotal),
                                                    out var charges)) charges = 0;

                                                auxTypeCost.Value = (allocation.Percentage / 100) * salary;
                                                auxTypeCost.OriginalValue = (allocation.Percentage / 100) * salary;
                                                auxTypeCost.Charges = (allocation.Percentage / 100) * charges;
                                                auxTypeCost.Adjustment = monthValue.Adjustment ?? 0;

                                                if (salary > 0)
                                                {
                                                    monthDetail.ChargesPercentage =
                                                        (decimal) ((auxTypeCost.Charges / auxTypeCost.Value) * 100);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (canViewSensibleData)
                            {
                                monthDetail.CanViewSensibleData = true;

                                if (employee.SocialCharges != null && employee.SocialCharges.Any())
                                {
                                    var socialCharge = employee?.SocialCharges.FirstOrDefault(x =>
                                        x.Year == mounth.MonthYear.Year && x.Month == mounth.MonthYear.Month);

                                    if (socialCharge != null)
                                    {
                                        var allocation = employee.Allocations.FirstOrDefault(x =>
                                            x.AnalyticId == IdAnalytic && x.StartDate.Date == mounth.MonthYear.Date);

                                        if (allocation != null)
                                        {
                                            if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.SalaryTotal),
                                                out var salary)) salary = 0;
                                            if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.ChargesTotal),
                                                out var charges)) charges = 0;

                                            auxTypeCost.Value = (allocation.Percentage / 100) * salary;
                                            auxTypeCost.OriginalValue = (allocation.Percentage / 100) * salary;
                                            auxTypeCost.Charges = (allocation.Percentage / 100) * charges;

                                            if (salary > 0)
                                            {
                                                monthDetail.ChargesPercentage =
                                                    (decimal) ((auxTypeCost.Charges / auxTypeCost.Value) * 100);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        switch (typeBudget.Name.ToUpper())
                        {
                            case EnumBudgetType.budget:
                                monthDetail.Budget = auxTypeCost;
                                break;
                            case EnumBudgetType.pfa1:
                                monthDetail.Pfa1 = auxTypeCost;
                                break;
                            case EnumBudgetType.pfa2:
                                monthDetail.Pfa2 = auxTypeCost;
                                break;
                            case EnumBudgetType.Real:
                                monthDetail.Real = auxTypeCost;
                                break;
                            case EnumBudgetType.Projected:
                                monthDetail.Projected = auxTypeCost;
                                break;
                        }

                        monthDetail.Closed = costDetailMonth.Closed;
                    }
                }

                monthDetail.Display = mounth.Display;
                monthDetail.MonthYear = mounth.MonthYear;
                monthDetail.Month = mounth.Month;
                monthDetail.Year = mounth.Year;

                //Verifico si este mes el recurso se encontro en la analitica
                if (employee.Allocations != null)
                {
                    var alocation = employee.Allocations.FirstOrDefault(x =>
                        x.AnalyticId == IdAnalytic && x.StartDate.Date == monthDetail.MonthYear.Date && x.Percentage > 0);

                    if (alocation != null)
                    {
                        monthDetail.HasAlocation = true;
                        monthDetail.AllocationPercentage = alocation.Percentage;
                    }
                    else
                    {
                        monthDetail.HasAlocation = false;
                    }

                    employeeHasAllocation = true;
                }
                else
                {
                    monthDetail.HasAlocation = false;
                }

                detailEmployee.MonthsCost.Add(monthDetail);
            }

            return detailEmployee;
        }

        private List<CostResource> FillFundedResoursesByMonth(IList<MonthHeaderCost> Months, ICollection<CostDetail> costDetails, List<CostDetailCategories> categories)
        {
            List<CostResource> fundedResources = new List<CostResource>();

            foreach (var category in categories)
            {
                var detailResource = new CostResource();
                detailResource.MonthsCost = new List<MonthDetailCost>();
                bool hasValue = false;

                detailResource.Display = category.Name;
                detailResource.TypeId = category.Id;
                detailResource.TypeName = category.Name;

                foreach (var mounth in Months)
                {
                    var monthDetail = new MonthDetailCost();

                    var costDetailMonth = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date).FirstOrDefault();
                    if (costDetailMonth != null)
                    {
                        var monthValue = costDetailMonth.CostDetailOthers.Where(o => o.CostDetailSubcategory.CostDetailCategoryId == category.Id && o.IsReal == false).ToList();
                        if (monthValue.Count > 0)
                        {
                            decimal valuePesos = 0;
                            foreach (var item in monthValue)
                            {
                                var currencyValue = mounth.CurrencyMonth.Where(x => x.CurrencyId == item.CurrencyId).FirstOrDefault();
                                if (currencyValue != null)
                                {
                                    valuePesos += item.Value * currencyValue.Exchange;
                                }
                                else
                                {
                                    valuePesos += item.Value;
                                }
                            }
                            // monthDetail.Budget.Value = monthValue.Sum(x => x.Value);
                            // monthDetail.CostDetailId = monthValue.FirstOrDefault().CostDetailId;
                            monthDetail.Budget.Value = valuePesos;
                            monthDetail.Budget.Id = monthValue.FirstOrDefault().Id;
                            if (monthDetail.Budget.Value > 0)
                            {
                                hasValue = true;
                            }
                        }

                        monthDetail.Closed = costDetailMonth.Closed;
                        monthDetail.CostDetailId = costDetailMonth.Id;
                    }

                    var monthValueReal = costDetailMonth.CostDetailOthers.Where(o => o.CostDetailSubcategory.CostDetailCategoryId == category.Id && o.IsReal == true).ToList();
                    if (monthValueReal.Count > 0)
                    {
                        monthDetail.Real.Id = monthValueReal.FirstOrDefault().Id;
                        monthDetail.Real.Value = monthValueReal.Sum(x => x.Value);
                        if (monthDetail.Real.Value > 0)
                        {
                            hasValue = true;
                        }
                    }

                    monthDetail.Display = mounth.Display;
                    monthDetail.MonthYear = mounth.MonthYear;
                    monthDetail.Month = mounth.Month;
                    monthDetail.Year = mounth.Year;

                    detailResource.MonthsCost.Add(monthDetail);
                }

                //Separo los campos por defectos de los ocultos
                //var typeWithoutValue = detailResource.MonthsCost.Where(m => m.Value == null || m.Value == 0).ToList();
                if (category.Default == false)
                {
                    detailResource.OtherResource = true;
                }

                if (category.BelongEmployee == true)
                {
                    detailResource.BelongEmployee = true;
                }

                if (category.Default == true || hasValue)
                {
                    detailResource.show = true;
                }


                fundedResources.Add(detailResource);
            }

            return fundedResources;
        }

        private List<CostProfile> FillProfilesByMonth(IList<MonthHeaderCost> Months, ICollection<CostDetail> costDetails)
        {
            var profilesResources = new List<CostProfile>();

            // Obtengo los perfiles del reporte.
            var profileIds = costDetails
                                .SelectMany(x => x.CostDetailProfiles.Select(p => new { ProfileId = p.EmployeeProfileId, Guid = p.Guid }))
                                .Distinct()
                                .ToList();

            var profiles = unitOfWork.UtilsRepository.GetEmployeeProfiles();

            foreach (var profileId in profileIds)
            {
                var detailProfile = new CostProfile();
                detailProfile.MonthsCost = new List<MonthDetailCost>();

                detailProfile.Display = profiles.Where(p => p.Id == profileId.ProfileId).FirstOrDefault().Text;
                detailProfile.EmployeeProfileId = profileId.ProfileId;
                detailProfile.Guid = profileId.Guid;
                detailProfile.TypeName = EnumCostDetailType.Profile;

                foreach (var mounth in Months)
                {
                    var monthDetail = new MonthDetailCost();

                    var costDetailMonth = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date).FirstOrDefault();
                    if (costDetailMonth != null)
                    {
                        var monthValue = costDetailMonth.CostDetailProfiles.FirstOrDefault(p => p.Guid == profileId.Guid);

                        if (monthValue != null)
                        {
                            monthDetail.Budget.Value = monthValue.Value;
                            monthDetail.Budget.Id = monthValue.Id;
                        }

                        monthDetail.Closed = costDetailMonth.Closed;
                    }

                    monthDetail.Display = mounth.Display;
                    monthDetail.MonthYear = mounth.MonthYear;
                    monthDetail.Month = mounth.Month;
                    monthDetail.Year = mounth.Year;

                    detailProfile.MonthsCost.Add(monthDetail);
                }


                profilesResources.Add(detailProfile);
            }

            return profilesResources;
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

        public void InsertUpdateCostDetailResources(IList<CostResourceEmployee> pCostEmployees, IList<CostDetail> costDetails, int managementReportId, bool isReal = false)
        {
            var managementReport = unitOfWork.ManagementReportRepository.GetWithAnalytic(managementReportId);

            managementReport.StartDate = managementReport.StartDate.AddDays(-1*(managementReport.StartDate.Day-1));

            try
            {
                foreach (var resource in pCostEmployees)
                {
                    var months = resource.MonthsCost.Where(x => x.MonthYear.Date >= managementReport.StartDate.Date && x.MonthYear.Date <= managementReport.EndDate.Date);

                    foreach (var month in months)
                    {
                        List<Cost> allBudgets = new List<Cost>();

                        allBudgets.Add(month.Budget);
                        allBudgets.Add(month.Real);

                        if (string.IsNullOrWhiteSpace(managementReport.Analytic.ServiceId))
                        {
                            allBudgets.Add(month.Pfa1);
                            allBudgets.Add(month.Pfa2);
                            allBudgets.Add(month.Projected);
                        }

                        foreach (var aux in allBudgets)
                        {
                            var entity = new CostDetailResource();

                            if (aux.Id > 0)
                            {
                                entity = unitOfWork.CostDetailResourceRepository.Get(aux.Id);

                                if (!decimal.TryParse(CryptographyHelper.Decrypt(entity.Value), out var salary)) salary = 0;
                                if (!decimal.TryParse(CryptographyHelper.Decrypt(entity.Charges), out var charges)) charges = 0;

                                if (aux.Value != salary || aux.Charges != charges)
                                {
                                    entity.Value = CryptographyHelper.Encrypt(aux.Value.ToString());
                                    entity.Adjustment = aux.Adjustment ?? 0;
                                    entity.Charges = CryptographyHelper.Encrypt(aux.Charges.ToString());

                                    unitOfWork.CostDetailResourceRepository.Update(entity);
                                }
                            }
                            else
                            {
                                if (aux.Value > 0 || aux.Charges > 0)
                                {
                                    entity.CostDetailId = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == month.MonthYear.Date).FirstOrDefault().Id;
                                    entity.Value = CryptographyHelper.Encrypt(aux.Value.ToString());
                                    entity.Adjustment = aux.Adjustment ?? 0;
                                    entity.Charges = CryptographyHelper.Encrypt(aux.Charges.ToString());
                                    entity.EmployeeId = resource.EmployeeId;
                                    entity.UserId = resource?.UserId;
                                    entity.BudgetTypeId = aux.BudgetTypeId;

                                    unitOfWork.CostDetailResourceRepository.Insert(entity);
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

        private void InsertUpdateCostDetailOther(IList<CostResource> pOtherResources, IList<CostDetail> costDetails, bool isReal = false)
        {
            try
            {
                foreach (var resource in pOtherResources)
                {
                    if (resource.TypeName.ToUpper() == EnumCostDetailType.AjusteGeneral.ToUpper())
                    {
                        resource.CurrencyId = appSetting.CurrencyPesos;
                    }

                    foreach (var month in resource.MonthsCost)
                    {
                        var entity = new CostDetailOther();

                        var aux = new Cost();


                        if (isReal)
                        {
                            aux.Id = month.Real.Id;
                            aux.Value = month.Real.Value;
                            aux.Description = month.Real.Description;
                        }
                        else
                        {
                            aux.Id = month.Budget.Id;
                            aux.Value = month.Budget.Value;
                            aux.Description = month.Budget.Description;
                        }

                        if (aux.Id > 0)
                        {
                            entity = unitOfWork.CostDetailOtherRepository.Get(aux.Id);

                            if (aux.Value != entity.Value || aux.Description != entity.Description || resource.CurrencyId != entity.CurrencyId)
                            {
                                entity.Value = aux.Value ?? 0;
                                entity.Description = aux.Description;
                                entity.CurrencyId = resource.CurrencyId;
                                entity.IsReal = isReal;

                                unitOfWork.CostDetailOtherRepository.Update(entity);
                            }
                        }
                        else
                        {
                            if (aux.Value > 0)
                            {
                                entity.CostDetailId = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == month.MonthYear.Date).FirstOrDefault().Id;
                                entity.Value = aux.Value ?? 0;
                                entity.Description = aux.Description;
                                entity.CostDetailSubcategoryId = resource.TypeId;
                                entity.CurrencyId = resource.CurrencyId;
                                entity.IsReal = isReal;

                                unitOfWork.CostDetailOtherRepository.Insert(entity);
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

        private void InsertUpdateCostDetailProfile(IList<CostProfile> pProfiles, IList<CostDetail> costDetails)
        {
            try
            {
                foreach (var profile in pProfiles)
                {
                    var guid = Guid.NewGuid().ToString();

                    foreach (var month in profile.MonthsCost)
                    {
                        var entity = new CostDetailProfile();

                        if (month.Budget.Id > 0)
                        {
                            entity = unitOfWork.CostDetailProfileRepository.Get(month.Budget.Id);

                            if (month.Budget.Value != entity.Value)
                            {
                                entity.Value = month.Budget.Value ?? 0;

                                unitOfWork.CostDetailProfileRepository.Update(entity);
                            }
                        }
                        else
                        {
                            if (month.Budget.Value > 0)
                            {
                                entity.Guid = guid;
                                entity.CostDetailId = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == month.MonthYear.Date).FirstOrDefault().Id;
                                entity.EmployeeProfileId = profile.EmployeeProfileId;
                                entity.Value = month.Budget.Value ?? 0;
                                entity.Description = profile.Description;

                                unitOfWork.CostDetailProfileRepository.Insert(entity);
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

        public IList<CostResourceEmployee> AddAnalyticMonthsToEmployees(IList<CostResourceEmployee> pCostEmployees, int managementReportId, DateTime startDateAnalytic, DateTime endDateAnalytic)
        {
            //Obtengo los valores de todos los meses de la analitica
            var resourcesCosts = unitOfWork.CostDetailResourceRepository.GetByAnalyticAndDates(managementReportId, startDateAnalytic, endDateAnalytic);

            foreach (var resource in pCostEmployees)
            {
                var lastMonthReport = resource.MonthsCost.OrderByDescending(x => x.MonthYear).FirstOrDefault();

                var dateLastMonthReport = new DateTime(lastMonthReport.MonthYear.Year, lastMonthReport.MonthYear.Month, 1);
                var dateEndDateAnalytic = new DateTime(endDateAnalytic.Year, endDateAnalytic.Month, 1);

                for (DateTime date = dateLastMonthReport.AddMonths(1); date.Date <= dateEndDateAnalytic; date = date.AddMonths(1))
                {
                    var month = new MonthDetailCost();

                    month.MonthYear = date;
                    month.Budget.Value = lastMonthReport.Budget.Value;
                    month.Budget.Charges = 0;
                    month.Budget.Adjustment = 0;

                    var exist = resourcesCosts
                                    .Where(e => e.EmployeeId == resource.EmployeeId
                                            && new DateTime(e.CostDetail.MonthYear.Year, e.CostDetail.MonthYear.Month, 1).Date == date.Date)
                                    .FirstOrDefault();

                    if (exist != null)
                    {
                        month.Budget.Id = exist.Id;
                    }

                    resource.MonthsCost.Add(month);
                }
            }

            return pCostEmployees;
        }

        private List<CostMonthOther> Translate(List<CostDetailOther> costDetailOthers)
        {
            return costDetailOthers
                    .Where(t => t.CostDetailSubcategory.Name != EnumCostDetailType.AjusteGeneral.ToString())
                    .Select(x => new CostMonthOther
                    {
                        Id = x.Id,
                        Description = x.Description,
                        CostDetailId = x.CostDetailId,
                        SubcategoryId = x.CostDetailSubcategoryId,
                        SubcategoryName = x.CostDetailSubcategory.Name,
                        CategoryName = x.CostDetailSubcategory.CostDetailCategory.Name,
                        CurrencyId = x.CurrencyId,
                        Value = x.Value
                    })
                    .OrderBy(x => x.SubcategoryName)
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

        private List<CostSubcategoryMonth> Translate(List<CostDetailSubcategories> subcategories)
        {
            return subcategories.Select(x => new CostSubcategoryMonth()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }

        public Response<ManagementReportCommentModel> AddComment(ManagementReportAddCommentModel model)
        {
            var response = new Response<ManagementReportCommentModel>();

            if (!unitOfWork.ManagementReportRepository.Exist(model.Id))
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            if (string.IsNullOrWhiteSpace(model.Comment))
            {
                response.AddError(Resources.ManagementReport.ManagementReport.CommentRequired);
                return response;
            }

            try
            {
                var mrComment = new ManagementReportComment();
                mrComment.Comment = model.Comment;
                mrComment.ManagementReportId = model.Id;
                mrComment.CreatedDate = DateTime.UtcNow;
                mrComment.UserName = userData.GetCurrentUser().UserName;

                unitOfWork.ManagementReportRepository.AddComment(mrComment);
                unitOfWork.Save();

                response.AddSuccess(Resources.ManagementReport.ManagementReport.CommentAdded);

                response.Data = new ManagementReportCommentModel
                {
                    Comment = mrComment.Comment,
                    Date = mrComment.CreatedDate.AddHours(-3),
                    UserName = mrComment.UserName
                };
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<ManagementReportCommentModel>> GetComments(int id)
        {
            var response = new Response<IList<ManagementReportCommentModel>> { Data = new List<ManagementReportCommentModel>() };

            var comments = unitOfWork.ManagementReportRepository.GetComments(id);

            if (comments.Any())
            {
                response.Data = comments.Select(x => new ManagementReportCommentModel
                {
                    Comment = x.Comment,
                    Date = x.CreatedDate.AddHours(-3),
                    UserName = x.UserName
                })
                .OrderByDescending(x => x.Date)
                .ToList();
            }

            return response;
        }

        public Response DeleteProfile(string guid)
        {
            var response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(guid))
                {
                    var entities = unitOfWork.CostDetailProfileRepository.Where
                                                                (x => x.Guid == guid);

                    unitOfWork.CostDetailProfileRepository.Delete(entities);
                }

                unitOfWork.Save();
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }
    }
}

