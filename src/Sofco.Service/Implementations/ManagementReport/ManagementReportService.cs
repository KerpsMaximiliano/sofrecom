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
                response.Data.ManagementReportId = analytic.ManagementReport.Id;
                response.Data.ManamementReportStartDate = analytic.ManagementReport.StartDate;
                response.Data.ManamementReportEndDate = analytic.ManagementReport.EndDate;
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

            var dates = SetDates(analytic);

            response.Data.MonthsHeader = new List<MonthBillingHeaderItem>();
            response.Data.Projects = new List<ProjectOption>();

            response.Data.ManagerId = analytic.Manager.ExternalManagerId;

            var costDetails = unitOfWork.CostDetailRepository.GetByManagementReportAndDates(analytic.ManagementReport.Id, dates.Item1.Date, dates.Item2.Date);
            var billings = unitOfWork.ManagementReportBillingRepository.GetByManagementReportAndDates(analytic.ManagementReport.Id, dates.Item1.Date, dates.Item2.Date);

            for (DateTime date = dates.Item1.Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
            {
                var monthHeader = new MonthBillingHeaderItem();
                monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                monthHeader.Year = date.Year;
                monthHeader.Month = date.Month;

                var costDetailMonth = costDetails.SingleOrDefault(x => x.MonthYear.Date == date.Date);

                if (costDetailMonth != null)
                    monthHeader.ResourceQuantity = costDetailMonth.CostDetailProfiles.Count + costDetailMonth.CostDetailResources.Count;

                var billingMonth = billings.SingleOrDefault(x => x.MonthYear.Date == date.Date);

                if (billingMonth != null)
                {
                    monthHeader.ValueEvalProp = billingMonth.EvalPropBillingValue;
                    monthHeader.BillingMonthId = billingMonth.Id;
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

                        var billingRowItem = new BillingHitoItem
                        {
                            Description = hito.Name,
                            Id = hito.Id,
                            ProjectId = project.CrmId,
                            ProjectName = $"{project.OpportunityNumber} {project.Name}",
                            CurrencyId = hito.MoneyId,
                            CurrencyName = hito.Money,
                            OpportunityNumber = project.OpportunityNumber,
                            Date = hito.StartDate,
                            MonthValues = new List<MonthBiilingRowItem>()
                        };

                        var rowItem = new MonthBiilingRowItem
                        {
                            Month = hito.StartDate.Month,
                            Year = hito.StartDate.Year,
                            Value = hito.Ammount,
                            OriginalValue = hito.AmountOriginal,
                            OriginalValuePesos = hito.BaseAmountOriginal,
                            Status = hito.Status
                        };

                        if (existHito != null)
                        {
                            rowItem.SolfacId = existHito.SolfacId;

                            if (existHito.Solfac.CurrencyExchange.HasValue && existHito.Solfac.CurrencyExchange > 0)
                            {
                                rowItem.ValuePesos = hito.Ammount * existHito.Solfac.CurrencyExchange.Value;
                            }
                            else
                            {
                                rowItem.ValuePesos = hito.BaseAmount;
                            }
                        }
                        else
                        {
                            rowItem.ValuePesos = hito.BaseAmount;
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
                var analytic = unitOfWork.AnalyticRepository.GetByServiceWithManagementReport(serviceId);

                if (analytic == null)
                {
                    response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                    return response;
                }

                response.Data.AnalyticId = analytic.Id;
                //Obtengo los meses que tiene la analitica
                response.Data.MonthsHeader = new List<MonthDetailCost>();

                response.Data.ManagerId = analytic.Manager.ExternalManagerId;
                response.Data.ManagementReportId = analytic.ManagementReport.Id;

                var dates = SetDates(analytic);

                var managementReport = unitOfWork.ManagementReportRepository.GetById(analytic.ManagementReport.Id);
                var costDetails = managementReport.CostDetails;
                var billings = unitOfWork.ManagementReportBillingRepository.GetByManagementReportAndDates(analytic.ManagementReport.Id, dates.Item1.Date, dates.Item2.Date);

                for (DateTime date = new DateTime(dates.Item1.Year, dates.Item1.Month, 1).Date; date.Date <= dates.Item2.Date; date = date.AddMonths(1))
                {
                    var monthHeader = new MonthDetailCost();
                    monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                    monthHeader.MonthYear = date;
                    monthHeader.Month = date.Month;
                    monthHeader.Year = date.Year;

                    var billingMonth = billings.SingleOrDefault(x => x.MonthYear.Date == date.Date);

                    if (billingMonth != null)
                    {
                        monthHeader.ValueEvalProp = billingMonth.EvalPropExpenseValue;
                        monthHeader.BillingMonthId = billingMonth.Id;
                    }

                    response.Data.MonthsHeader.Add(monthHeader);
                }

                //Obtengo los tipos de Recursos
                List<CostDetailType> Types = unitOfWork.CostDetailRepository.GetResourceTypes();

                //Mapeo Los demas datos
                var AllCostResources = FillFundedResoursesByMonth(response.Data.MonthsHeader, costDetails, Types);
                //Mapeo Los empleados      

                response.Data.CostEmployees = FillCostEmployeesByMonth(analytic.Id, response.Data.MonthsHeader, costDetails);
                response.Data.FundedResources = AllCostResources.Where(r => r.show == true).ToList();
                response.Data.OtherResources = AllCostResources.Where(r => r.show == false).OrderBy(r => r.Display).ToList();
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

                List<ContractedModel> listContracted = costDetail.ContratedDetails
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

                List<CostMonthOther> listOther = costDetail.CostDetailOthers
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

                //List<CostMonthEmployee> listEmployees = costDetail.CostDetailResources
                //                                         .Select(empl => new CostMonthEmployee
                //                                         {
                //                                             Id = empl.Id,
                //                                             EmployeeId = empl.EmployeeId,
                //                                             UserId = empl.UserId,
                //                                             CostDetailId = empl.CostDetailId,
                //                                             Salary = empl.Value,
                //                                             Charges = empl.Charges
                //                                         })
                //                                         .ToList();

                response.Data.ManagementReportId = analytic.ManagementReport.Id;
                response.Data.MonthYear = costDetail.MonthYear;
                response.Data.Contracted = listContracted;
                response.Data.OtherResources = listOther;
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.Messages.Add(new Message(Resources.Common.GeneralError, MessageType.Error));
            }

            return response;
        }

        public Response<List<CostDetailTypeModel>> GetOtherResources()
        {
            var response = new Response<List<CostDetailTypeModel>> { Data = new List<CostDetailTypeModel>() };

            try
            {
                List<CostDetailType> Types = unitOfWork.CostDetailRepository.GetResourceTypes();

                response.Data = Types
                                     .Where(t => t.Name != EnumCostDetailType.AjusteGeneral.ToString())
                                     .Select(t => new CostDetailTypeModel
                                     {
                                         TypeId = t.Id,
                                         TypeName = t.Name
                                     })
                                     .OrderBy(t => t.TypeName)
                                     .ToList();
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

                this.VerifyMonthsCostDetail(pDetailCost.MonthsHeader, managementReport);
                this.InsertUpdateCostDetailResources(pDetailCost.CostEmployees, managementReport.CostDetails.ToList());
                this.InsertUpdateCostDetailOther(pDetailCost.FundedResources, managementReport.CostDetails.ToList());
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

            try
            {
                foreach (var employee in pMonthDetail.Employees)
                {
                    var cost = new CostResourceEmployee();
                    cost.MonthsCost = new List<MonthDetailCost>();
                    MonthDetailCost month = new MonthDetailCost();

                    cost.EmployeeId = employee.EmployeeId;
                    cost.UserId = employee.UserId;

                    month.Value = employee.Salary;
                    month.Charges = employee.Charges;
                    month.MonthYear = pMonthDetail.MonthYear;
                    month.Id = employee.Id;

                    cost.MonthsCost.Add(month);
                    _detailModel.CostEmployees.Add(cost);
                }

                foreach (var otherRes in pMonthDetail.OtherResources)
                {
                    CostResource cost = new CostResource();
                    cost.MonthsCost = new List<MonthDetailCost>();
                    MonthDetailCost month = new MonthDetailCost();

                    cost.TypeId = otherRes.TypeId;
                    month.Value = otherRes.Value;
                    month.MonthYear = pMonthDetail.MonthYear;
                    month.Id = otherRes.Id;
                    month.Description = otherRes.Description;

                    cost.MonthsCost.Add(month);
                    _detailModel.FundedResources.Add(cost);
                }

                var costDetails = unitOfWork.CostDetailRepository.GetByManagementReport(pMonthDetail.ManagementReportId);

                this.InsertUpdateCostDetailResources(_detailModel.CostEmployees, costDetails);
                this.InsertUpdateCostDetailOther(_detailModel.FundedResources, costDetails);
                this.UpdateContracted(pMonthDetail.Contracted, costDetails, pMonthDetail.MonthYear);

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

        public Response UpdateDates(int id, ManagementReportUpdateDates model)
        {
            var response = new Response();

            var managementReport = unitOfWork.ManagementReportRepository.Get(id);

            if (managementReport == null)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.NotFound);
                return response;
            }

            if (!model.StartDate.HasValue) response.AddError(Resources.ManagementReport.ManagementReport.StartDateRequired);
            if (!model.EndDate.HasValue) response.AddError(Resources.ManagementReport.ManagementReport.EndDateRequired);

            if (model.StartDate.GetValueOrDefault().Date > model.EndDate.GetValueOrDefault().Date)
            {
                response.AddError(Resources.ManagementReport.ManagementReport.StartDateGreaterThanEndDate);
                return response;
            }

            if (response.HasErrors()) return response;

            try
            {
                managementReport.StartDate = model.StartDate.GetValueOrDefault().Date;
                managementReport.EndDate = model.EndDate.GetValueOrDefault().Date;

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

        private List<CostResourceEmployee> FillCostEmployeesByMonth(int IdAnalytic, IList<MonthDetailCost> Months, ICollection<CostDetail> costDetails)
        {
            List<CostResourceEmployee> costEmployees = new List<CostResourceEmployee>();

            //Obtengo los empleados de la analitica
            var EmployeesAnalytic = unitOfWork.EmployeeRepository.GetByAnalyticWithWorkTimes(IdAnalytic);

            // Obtengo los empleados del reporte que no estan en la analitica.
            var IdEmployeesWithOutAnalytic = costDetails
                                                .SelectMany(x => x.CostDetailResources.Select(d => d.EmployeeId))
                                                .Distinct()
                                                .Except(EmployeesAnalytic.Select(x => x.Id))
                                                .ToArray();

            var employeesWithOutAnalytic = unitOfWork.EmployeeRepository.GetById(IdEmployeesWithOutAnalytic);

            var allEmployees = EmployeesAnalytic.Union(employeesWithOutAnalytic).ToList();

            foreach (var employee in allEmployees)
            {
                var user = unitOfWork.UserRepository.GetByEmail(employee.Email);

                var detailEmployee = new CostResourceEmployee();
                detailEmployee.MonthsCost = new List<MonthDetailCost>();

                detailEmployee.EmployeeId = employee.Id;
                detailEmployee.UserId = user?.Id;
                detailEmployee.Display = employee.Name + " - " + employee.EmployeeNumber;
                //detailEmployee.TypeId = EmployeeType.Id;
                detailEmployee.TypeName = EnumCostDetailType.Empleados.ToString();

                foreach (var mounth in Months)
                {
                    var monthDetail = new MonthDetailCost();

                    var costDetailMonth = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date).FirstOrDefault();
                    if (costDetailMonth != null)
                    {
                        var monthValue = costDetailMonth.CostDetailResources.FirstOrDefault(e => e.EmployeeId == employee.Id);
                        // var monthValue = CostDetailEmployees.Find(e => e.CostDetailResources.EmployeeId == employee.Id && new DateTime(e.MonthYear.Year, e.MonthYear.Month, 1).Date == mounth.MonthYear.Date);
                        if (monthValue != null)
                        {
                            monthDetail.Value = monthValue.Value;
                            monthDetail.OriginalValue = monthValue.Value;
                            monthDetail.Charges = monthValue.Charges;
                            monthValue.Adjustment = monthValue.Adjustment;
                            monthDetail.Id = monthValue.Id;
                        }
                    }

                    monthDetail.Display = mounth.Display;
                    monthDetail.MonthYear = mounth.MonthYear;

                    //Verifico si este mes el recurso se encontro en la analitica
                    var startDate = new DateTime(mounth.MonthYear.Year, mounth.MonthYear.Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    if (employee.Allocations != null)
                    {
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
                    }

                    detailEmployee.MonthsCost.Add(monthDetail);
                }
                costEmployees.Add(detailEmployee);
            }

            return costEmployees.OrderBy(e => e.Display).ToList();
        }

        private List<CostResource> FillFundedResoursesByMonth(IList<MonthDetailCost> Months, ICollection<CostDetail> costDetails, List<CostDetailType> Types)
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

                    var costDetailMonth = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date).FirstOrDefault();
                    if (costDetailMonth != null)
                    {
                        var monthValue = costDetailMonth.CostDetailOthers.Where(o => o.CostDetailTypeId == type.Id).ToList();
                        if (monthValue.Count > 0)
                        {
                            monthDetail.Value = monthValue.Sum(x => x.Value);
                            monthDetail.CostDetailId = monthValue.FirstOrDefault().CostDetailId;
                            // Cambiar.
                            monthDetail.Id = monthValue.FirstOrDefault().Id;
                            if (monthDetail.Value > 0)
                            {
                                hasValue = true;
                            }
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

        private List<CostProfile> FillProfilesByMonth(IList<MonthDetailCost> Months, ICollection<CostDetail> costDetails)
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

                foreach (var mounth in Months)
                {
                    var monthDetail = new MonthDetailCost();

                    var costDetailMonth = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == mounth.MonthYear.Date).FirstOrDefault();
                    if (costDetailMonth != null)
                    {
                        var monthValue = costDetailMonth.CostDetailProfiles.FirstOrDefault(p => p.Guid == profileId.Guid);

                        if (monthValue != null)
                        {
                            monthDetail.Value = monthValue.Value;
                            monthDetail.Id = monthValue.Id;
                        }
                    }

                    monthDetail.Display = mounth.Display;
                    monthDetail.MonthYear = mounth.MonthYear;
                    detailProfile.MonthsCost.Add(monthDetail);
                }


                profilesResources.Add(detailProfile);
            }

            return profilesResources;
        }

        private void VerifyMonthsCostDetail(IList<MonthDetailCost> pMonths, Sofco.Domain.Models.ManagementReport.ManagementReport pManagementReport)
        {
            // Verifico que todos los meses del reporte esten cargados en base de datos.
            try
            {
                foreach (var mounth in pMonths)
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
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                throw ex;
            }
        }

        private void InsertUpdateCostDetailResources(IList<CostResourceEmployee> pCostEmployees, IList<CostDetail> costDetails)
        {
            try
            {
                foreach (var resource in pCostEmployees)
                {
                    foreach (var month in resource.MonthsCost)
                    {
                        var entity = new CostDetailResource();

                        if (month.Id > 0)
                        {
                            entity = unitOfWork.CostDetailResourceRepository.Get(month.Id);

                            if (month.Value != entity.Value || month.Charges != entity.Charges)
                            {
                                entity.Value = month.Value ?? 0;
                                entity.Adjustment = month.Adjustment ?? 0;
                                entity.Charges = month.Charges ?? 0;

                                unitOfWork.CostDetailResourceRepository.Update(entity);
                            }
                        }
                        else
                        {
                            if (month.Value > 0 || month.Charges > 0)
                            {
                                entity.CostDetailId = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == month.MonthYear.Date).FirstOrDefault().Id;
                                entity.Value = month.Value ?? 0;
                                entity.Adjustment = month.Adjustment ?? 0;
                                entity.Charges = month.Charges ?? 0;
                                entity.EmployeeId = resource.EmployeeId;
                                entity.UserId = resource?.UserId;

                                unitOfWork.CostDetailResourceRepository.Insert(entity);
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

        private void InsertUpdateCostDetailOther(IList<CostResource> pOtherResources, IList<CostDetail> costDetails)
        {
            try
            {
                foreach (var resource in pOtherResources)
                {
                    foreach (var month in resource.MonthsCost)
                    {
                        var entity = new CostDetailOther();

                        if (month.Id > 0)
                        {
                            entity = unitOfWork.CostDetailOtherRepository.Get(month.Id);

                            if (month.Value != entity.Value || month.Description != entity.Description)
                            {
                                entity.Value = month.Value ?? 0;
                                entity.Description = month.Description;

                                unitOfWork.CostDetailOtherRepository.Update(entity);
                            }
                        }
                        else
                        {
                            if (month.Value > 0)
                            {
                                entity.CostDetailId = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == month.MonthYear.Date).FirstOrDefault().Id;
                                entity.Value = month.Value ?? 0;
                                entity.Description = resource.Description;
                                entity.CostDetailTypeId = resource.TypeId;

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

                        if (month.Id > 0)
                        {
                            entity = unitOfWork.CostDetailProfileRepository.Get(month.Id);

                            if (month.Value != entity.Value)
                            {
                                entity.Value = month.Value ?? 0;

                                unitOfWork.CostDetailProfileRepository.Update(entity);
                            }
                        }
                        else
                        {
                            if (month.Value > 0)
                            {
                                entity.Guid = guid;
                                entity.CostDetailId = costDetails.Where(c => new DateTime(c.MonthYear.Year, c.MonthYear.Month, 1).Date == month.MonthYear.Date).FirstOrDefault().Id;
                                entity.EmployeeProfileId = profile.EmployeeProfileId;
                                entity.Value = month.Value ?? 0;
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
    }
}

