using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Services.ManagementReport;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AllocationService : IAllocationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<AllocationService> logger;
        private readonly IAllocationFileManager allocationFileManager;
        private readonly ILicenseGenerateWorkTimeService licenseGenerateWorkTimeService;
        private readonly IManagementReportCalculateCostsService managementReportCalculateCostsService;

        public AllocationService(IUnitOfWork unitOfWork,
            ILogMailer<AllocationService> logger,
            ILicenseGenerateWorkTimeService licenseGenerateWorkTimeService,
            IManagementReportCalculateCostsService managementReportCalculateCostsService,
            IAllocationFileManager allocationFileManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.allocationFileManager = allocationFileManager;
            this.licenseGenerateWorkTimeService = licenseGenerateWorkTimeService;
            this.managementReportCalculateCostsService = managementReportCalculateCostsService;
        }

        public Response<Allocation> Add(AllocationDto allocation)
        {
            var response = new Response<Allocation>();

            AnalyticValidationHelper.Exist(response, unitOfWork.AnalyticRepository, allocation.AnalyticId);
            EmployeeValidationHelper.Exist(response, unitOfWork.EmployeeRepository, allocation.EmployeeId);
            AllocationValidationHelper.ValidatePercentage(response, allocation);
            AllocationValidationHelper.ValidateReleaseDate(response, allocation);

            if (response.HasErrors()) return response;

            var firstMonth = allocation.Months.FirstOrDefault();
            var lastMonth = allocation.Months[allocation.Months.Count - 1];

            var allocationsBetweenDays = unitOfWork.AllocationRepository.GetAllocationsBetweenDays(allocation.EmployeeId, firstMonth.Date.Date, lastMonth.Date.Date);

            if (allocationsBetweenDays.Count > 0)
            {
                AllocationValidationHelper.ValidatePercentageRange(response, allocationsBetweenDays, allocation);

                if (response.HasErrors()) return response;

                SaveAllocation(allocation, response, allocationsBetweenDays);
            }
            else
            {
                SaveAllocation(allocation, response, allocationsBetweenDays);
            }

            //managementReportCalculateCostsService.CalculateCosts(allocation, firstMonth.Date, lastMonth.Date);

            var licenses = unitOfWork.LicenseRepository.GetByEmployeeAndDates(allocation.EmployeeId, firstMonth.Date.Date, lastMonth.Date.Date);

            if (licenses.Any())
            {
                foreach (var license in licenses)
                {
                    try
                    {
                        unitOfWork.WorkTimeRepository.RemoveBetweenDays(license.EmployeeId, license.StartDate, license.EndDate);
                        licenseGenerateWorkTimeService.GenerateWorkTimes(license);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e);
                        response.AddWarning(Resources.Rrhh.License.GenerateWorkTimesError);
                    }
                }

                unitOfWork.Save();
            }

            return response;
        }

        public AllocationResponse GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate, IList<int> analyticIdsParameter)
        {
            var allocations = unitOfWork.AllocationRepository.GetAllocationsBetweenDays(employeeId, startDate, endDate);
            var licenses = unitOfWork.LicenseRepository.GetByEmployeeAndDates(employeeId, startDate, endDate);

            var allocationResponse = new AllocationResponse();

            BuildMonthHeader(startDate, endDate, licenses, allocationResponse);

            if (licenses.Any())
            {
                allocationResponse.Licenses = licenses.Select(x => new LicenseItemDto(x)).ToList();
            }

            var analyticsIds = allocations.Select(x => x.AnalyticId).Distinct();

            var diccionary = new Dictionary<DateTime, decimal>();

            foreach (var analyticId in analyticsIds)
            {
                if (analyticIdsParameter.Any() && !analyticIdsParameter.Contains(analyticId)) continue;

                var allocationDto = new AllocationDto();
                allocationDto.EmployeeId = employeeId;

                var allocation = allocations.Where(x => x.AnalyticId == analyticId).ToList();

                //if (allocation.All(x => x.Percentage == 0)) continue;

                if (allocation.Any())
                {
                    var first = allocation.FirstOrDefault();

                    allocationDto.AnalyticId = analyticId;
                    allocationDto.AnalyticTitle = first.Analytic.Title;
                    allocationDto.Id = first.Id;
                }
                else
                {
                    allocationDto.AnalyticId = 0;
                    allocationDto.AnalyticTitle = "Sin Asignación";
                    allocationDto.Id = 0;
                }

                for (DateTime date = startDate.Date; date.Date <= endDate.Date; date = date.AddMonths(1))
                {
                    var allocationMonth = allocation.FirstOrDefault(x => x.StartDate.Date == date.Date);

                    var allocationMonthDto = new AllocationMonthDto();
                    allocationMonthDto.AllocationId = allocationMonth?.Id ?? 0;
                    allocationMonthDto.Date = date;
                    allocationMonthDto.Percentage = allocationMonth?.Percentage ?? 0;
                    allocationMonthDto.ReleaseDate = allocationMonth?.ReleaseDate.Date.Date ?? DateTime.UtcNow.Date;

                    if (diccionary.ContainsKey(date))
                    {
                        diccionary[date] += allocationMonthDto.Percentage.GetValueOrDefault();
                    }
                    else
                    {
                        diccionary.Add(date, allocationMonthDto.Percentage.GetValueOrDefault());
                    }

                    if (!allocationDto.ReleaseDate.HasValue)
                    {
                        allocationDto.ReleaseDate = allocationMonth?.ReleaseDate.Date == DateTime.MinValue ? null : allocationMonth?.ReleaseDate.Date;
                    }

                    allocationDto.Months.Add(allocationMonthDto);
                }

                allocationResponse.Allocations.Add(allocationDto);
            }

            foreach (var allocationDto in allocationResponse.Allocations)
            {
                foreach (var allocationMonthDto in allocationDto.Months)
                {
                    allocationMonthDto.PercentageSum = diccionary[allocationMonthDto.Date];
                }
            }

            return allocationResponse;
        }

        private void BuildMonthHeader(DateTime startDate, DateTime endDate, ICollection<Domain.Models.Rrhh.License> licenses, AllocationResponse allocationResponse)
        {
            for (DateTime date = startDate.Date; date.Date <= endDate.Date; date = date.AddMonths(1))
            {
                var monthHeader = new MonthHeader();
                monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                monthHeader.Month = date.Month;
                monthHeader.Year = date.Year;
                monthHeader.EmployeeHasLicense = licenses.Any(x => (x.StartDate.Month == date.Month &&x.StartDate.Year == date.Year) || (x.EndDate.Month == date.Month && x.EndDate.Year == date.Year));

                allocationResponse.MonthsHeader.Add(monthHeader);
            }
        }

        private IList<AllocationDateReport> GetDateForReport(DateTime startDate, DateTime endDate)
        {
            var list = new List<AllocationDateReport>();

            for (DateTime date = startDate.Date; date.Date <= endDate.Date; date = date.AddMonths(1))
            {
                var monthHeader = new AllocationDateReport();
                monthHeader.Month = date.Month;
                monthHeader.Year = date.Year;
                monthHeader.Percentage = 0;

                list.Add(monthHeader);
            }

            return list;
        }

        private IList<string> GetDateHeaderForReport(DateTime startDate, DateTime endDate)
        {
            var list = new List<string>();

            for (DateTime date = startDate.Date; date.Date <= endDate.Date; date = date.AddMonths(1))
            {
                list.Add(DatesHelper.GetDateShortDescription(date));
            }

            return list;
        }

        public ICollection<Employee> GetByService(string serviceId)
        {
            return unitOfWork.AllocationRepository.GetByService(serviceId);
        }

        public ICollection<Employee> GetByEmployeesByAnalytic(int analyticId)
        {
            return unitOfWork.AllocationRepository.GetByAnalyticId(analyticId);
        }

        public Response<byte[]> AddMassive(AllocationMassiveAddModel model)
        {
            var response = new Response<byte[]>();
            var employeesWithError = new List<Tuple<string, string, decimal>>();

            AnalyticValidationHelper.Exist(response, unitOfWork.AnalyticRepository, model.AnalyticId);
            AllocationValidationHelper.ValidatePercentage(response, model);
            AllocationValidationHelper.ValidateDates(response, model);

            if (response.HasErrors()) return response;

            var firstMonth = new DateTime(model.StartDate.GetValueOrDefault().Year, model.StartDate.GetValueOrDefault().Month, 1);
            var lastMonth = new DateTime(model.EndDate.GetValueOrDefault().Year, model.EndDate.GetValueOrDefault().Month, 1);

            try
            {
                foreach (var employeeId in model.EmployeeIds)
                {
                    var allocationsBetweenDays = unitOfWork.AllocationRepository.GetAllocationsBetweenDays(employeeId, firstMonth.Date, lastMonth.Date);
                    var firstMonthAux = firstMonth.Date;

                    while (firstMonthAux.Date <= lastMonth.Date)
                    {
                        var allocationsFiltered = allocationsBetweenDays.Where(x => x.StartDate.Date == firstMonthAux.Date).ToList();

                        if (allocationsFiltered.Any())
                        {
                            //Si existe Asignacion anterior la elimino e inserto la asignacion nueva
                            unitOfWork.AllocationRepository.Delete(allocationsFiltered);
                            InsertNewAllocation(model, employeeId, firstMonthAux);
                        }
                        else
                        {
                            InsertNewAllocation(model, employeeId, firstMonthAux);
                        }

                        firstMonthAux = firstMonthAux.AddMonths(1);
                    }

                    unitOfWork.Save();

                    //Licencias
                    var licenses = unitOfWork.LicenseRepository.GetByEmployeeAndDates(employeeId, firstMonth.Date.Date, model.EndDate.GetValueOrDefault());
                    if (licenses.Any())
                    {
                        foreach (var license in licenses)
                        {
                            try
                            {
                                unitOfWork.WorkTimeRepository.RemoveBetweenDays(license.EmployeeId, license.StartDate, license.EndDate);
                                licenseGenerateWorkTimeService.GenerateWorkTimes(license);
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e);
                                response.AddWarning(Resources.Rrhh.License.GenerateWorkTimesError);
                            }
                        }

                        unitOfWork.Save();
                    }
                }

                if (employeesWithError.Any())
                {
                    response.Data = allocationFileManager.CreateReport(employeesWithError).GetAsByteArray();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void InsertNewAllocation(AllocationMassiveAddModel model, int employeeId, DateTime firstMonthAux)
        {
            var allocation = new Allocation
            {
                Id = 0,
                AnalyticId = model.AnalyticId,
                StartDate = firstMonthAux.Date,
                Percentage = model.Percentage.GetValueOrDefault(),
                EmployeeId = employeeId,
                ReleaseDate = model.EndDate.GetValueOrDefault().Date
            };

            unitOfWork.AllocationRepository.Insert(allocation);
        }

        public Response<AllocationReportModel> CreateReport(AllocationReportParams parameters)
        {
            var response = new Response<AllocationReportModel> { Data = new AllocationReportModel() };

            if (!parameters.StartDate.HasValue)
                response.AddError(Resources.AllocationManagement.Allocation.DateSinceRequired);

            if (!parameters.EndDate.HasValue)
                response.AddError(Resources.AllocationManagement.Allocation.DateToRequired);

            if (response.HasErrors()) return response;

            if (parameters.StartDate.GetValueOrDefault().Date > parameters.EndDate.GetValueOrDefault().Date)
            {
                response.AddError(Resources.AllocationManagement.Allocation.DateToLessThanDateSince);
                return response;
            }

            parameters.StartDate = new DateTime(parameters.StartDate.GetValueOrDefault().Year, parameters.StartDate.GetValueOrDefault().Month, 1);

            parameters.EndDate = new DateTime(parameters.EndDate.GetValueOrDefault().Year, parameters.EndDate.GetValueOrDefault().Month,
                                 DateTime.DaysInMonth(parameters.EndDate.GetValueOrDefault().Year, parameters.EndDate.GetValueOrDefault().Month));

            response.Data.MonthsHeader = GetDateHeaderForReport(parameters.StartDate.Value, parameters.EndDate.Value);

            var months = GetDateForReport(parameters.StartDate.Value, parameters.EndDate.Value);

            var employees = unitOfWork.AllocationRepository.GetByEmployeesForReport(parameters);

            var listWithoutMissingPercentage = new List<AllocationReportRow>();
  
            foreach (var employee in employees)
            {
                var startDate = parameters.StartDate.GetValueOrDefault();
                var endDate = parameters.EndDate.GetValueOrDefault();

                while (startDate.Date <= endDate.Date)
                {
                    var date = startDate;

                    var allocations = employee.Allocations.Where(x => x.StartDate == date);

                    var percentageSum = allocations.Sum(x => x.Percentage);

                    if (percentageSum < 100)
                    {
                        if (!(!parameters.Unassigned && parameters.AnalyticIds.Any()))
                        {
                            AddUnassginRow(listWithoutMissingPercentage, employee, months, 100 - percentageSum, date);
                        }
                    }

                    if (!parameters.Unassigned)
                    {
                        if (parameters.AnalyticIds.Any())
                            allocations = allocations.Where(x => parameters.AnalyticIds.Contains(x.AnalyticId));

                        if (parameters.IncludeAnalyticId == 2)
                            allocations = allocations.Where(x => x.Analytic.Status == AnalyticStatus.Open);

                        if (parameters.IncludeAnalyticId == 3)
                            allocations = allocations.Where(x => x.Analytic.Status == AnalyticStatus.Close || x.Analytic.Status == AnalyticStatus.CloseToExpenses);
                    }

                    if (!parameters.Unassigned)
                    {
                        foreach (var allocation in allocations.ToList())
                        {
                            if (allocation.Percentage > 0)
                            {
                                AddRowReport(response, allocation, months);
                            }
                        }
                    }

                    startDate = startDate.AddMonths(1);
                }
            }

            foreach (var allocationReportRow in listWithoutMissingPercentage)
            {
                response.Data.Rows.Add(allocationReportRow);
            }

            IList<Employee> employeesUnassigned = new List<Employee>();

            if (parameters.Unassigned)
            {
                employeesUnassigned = unitOfWork.EmployeeRepository.GetUnassignedBetweenDays(parameters.StartDate.Value, parameters.EndDate.Value);
            }
            else
            {
                if (!parameters.AnalyticIds.Any() &&
                    (parameters.IncludeAnalyticId == 1 || parameters.IncludeAnalyticId == 2) &&
                    !parameters.EmployeeId.HasValue || (parameters.EmployeeId.HasValue && parameters.EmployeeId.Value == 0))
                {
                    employeesUnassigned = unitOfWork.EmployeeRepository.GetUnassignedBetweenDays(parameters.StartDate.Value, parameters.EndDate.Value);
                }
            }

            foreach (var employee in employeesUnassigned)
            {
                var reportRow = new AllocationReportRow
                {
                    Manager = employee.Manager?.Name,
                    Percentage = employee.BillingPercentage,
                    Profile = employee.Profile,
                    EmployeeNumber = employee.EmployeeNumber,
                    ResourceName = employee.Name,
                    Seniority = employee.Seniority,
                    Technology = employee.Technology,
                    Analytic = "Sin Asignación",
                    EmployeeId = employee.Id,
                    AnalyticId = 0,
                    Months = months.Select(x => new AllocationDateReport { Year = x.Year, Month = x.Month, Percentage = 100 }).ToList()
                };

                response.Data.Rows.Add(reportRow);
            }

            if (!response.Data.Rows.Any())
            {
                response.AddWarning(Resources.AllocationManagement.Employee.EmployeesNotFound);
            }

            return response;
        }

        private void AddUnassginRow(IList<AllocationReportRow> list, Employee employee, IList<AllocationDateReport> months, decimal percentageDiff, DateTime startDate)
        {
            var row = list.FirstOrDefault(x => x.EmployeeId == employee.Id);
            var newRow = false;

            if (row == null)
            {
                newRow = true;

                row = new AllocationReportRow
                {
                    AnalyticId = 0,
                    EmployeeId = employee.Id,
                    Manager = employee.Manager?.Name,
                    Percentage = employee.BillingPercentage,
                    Profile = employee.Profile,
                    ResourceName = employee.Name,
                    EmployeeNumber = employee.EmployeeNumber,
                    Seniority = employee.Seniority,
                    Technology = employee.Technology,
                    Analytic = "Sin Asignación",
                    Months = months.Select(x => new AllocationDateReport(x)).ToList()
                };
            }

            var month = row.Months.FirstOrDefault(x => x.Year == startDate.Year && x.Month == startDate.Month);

            if (month != null) month.Percentage = percentageDiff;

            if (newRow)
            {
                list.Add(row);
            }
        }

        private void AddRowReport(Response<AllocationReportModel> response, Allocation allocation, IList<AllocationDateReport> months)
        {
            var row = response.Data.Rows.FirstOrDefault(x => x.EmployeeId == allocation.EmployeeId && x.AnalyticId == allocation.AnalyticId);
            var newRow = false;

            if (row == null)
            {
                newRow = true;

                row = new AllocationReportRow
                {
                    AnalyticId = allocation.AnalyticId,
                    EmployeeId = allocation.EmployeeId,
                    Manager = allocation.Employee.Manager?.Name,
                    Percentage = allocation.Employee.BillingPercentage,
                    Profile = allocation.Employee.Profile,
                    ResourceName = allocation.Employee.Name,
                    EmployeeNumber = allocation.Employee.EmployeeNumber,
                    Seniority = allocation.Employee.Seniority,
                    Technology = allocation.Employee.Technology,
                    AnalyticTitle = allocation.Analytic?.Title,
                    Analytic = allocation.Analytic?.Name,
                    Months = months.Select(x => new AllocationDateReport(x)).ToList()
                };
            }

            var month = row.Months.FirstOrDefault(x => x.Year == allocation.StartDate.Year && x.Month == allocation.StartDate.Month);

            if (month != null) month.Percentage = allocation.Percentage;

            if (newRow)
            {
                response.Data.Rows.Add(row);
            }
        }

        public IEnumerable<OptionPercentage> GetAllPercentages()
        {
            yield return new OptionPercentage { Id = (int)AllocationPercentage.Differente100, Text = AllocationPercentage.Differente100.ToString(), StartValue = 0, EndValue = 99 };
            yield return new OptionPercentage { Id = (int)AllocationPercentage.Equals100, Text = AllocationPercentage.Equals100.ToString(), StartValue = 100, EndValue = 100 };
            yield return new OptionPercentage { Id = (int)AllocationPercentage.Different0, Text = AllocationPercentage.Different0.ToString(), StartValue = 1, EndValue = 100 };
            yield return new OptionPercentage { Id = (int)AllocationPercentage.Equals0, Text = AllocationPercentage.Equals0.ToString(), StartValue = 0, EndValue = 0 };
            yield return new OptionPercentage { Id = (int)AllocationPercentage.Between0And50, Text = AllocationPercentage.Between0And50.ToString(), StartValue = 0, EndValue = 50 };
            yield return new OptionPercentage { Id = (int)AllocationPercentage.Between50And75, Text = AllocationPercentage.Between50And75.ToString(), StartValue = 50, EndValue = 75 };
            yield return new OptionPercentage { Id = (int)AllocationPercentage.Between75And99, Text = AllocationPercentage.Between75And99.ToString(), StartValue = 75, EndValue = 99 };
        }

        private void SaveAllocation(AllocationDto allocationDto, Response response, ICollection<Allocation> allocationsBetweenDays)
        {
            try
            {
                foreach (var month in allocationDto.Months)
                {
                    if (month.AllocationId > 0)
                    {
                        var allocation = allocationsBetweenDays.SingleOrDefault(x => x.Id == month.AllocationId);

                        if (allocation != null)
                        {
                            if (month.Updated)
                            {
                                allocation.Percentage = month.Percentage.GetValueOrDefault();
                                unitOfWork.AllocationRepository.UpdatePercentage(allocation);
                            }

                            allocation.ReleaseDate = allocationDto.ReleaseDate.GetValueOrDefault().Date;
                            unitOfWork.AllocationRepository.UpdateReleaseDate(allocation);
                        }
                        else
                        {
                            InsertNewAllocation(allocationDto, month);
                        }
                    }
                    else
                    {
                        var allocation = unitOfWork.AllocationRepository.GetSingle(x => x.AnalyticId == allocationDto.AnalyticId &&
                                                                                        x.EmployeeId == allocationDto.EmployeeId &&
                                                                                        x.StartDate.Date == month.Date.Date);

                        if (allocation == null)
                        {
                            InsertNewAllocation(allocationDto, month);
                        }
                        else
                        {
                            allocation.Percentage = month.Percentage.GetValueOrDefault();
                            unitOfWork.AllocationRepository.UpdatePercentage(allocation);
                            allocation.ReleaseDate = allocationDto.ReleaseDate.GetValueOrDefault().Date;
                            unitOfWork.AllocationRepository.UpdateReleaseDate(allocation);
                        }
                    }
                }

                unitOfWork.Save();

                unitOfWork.AllocationRepository.DeleteAllocationWithReleaseDateNull();

                response.AddSuccess(Resources.AllocationManagement.Allocation.Added);
            }
            catch (Exception ex)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(ex);
            }
        }

        private void InsertNewAllocation(AllocationDto allocationDto, AllocationMonthDto month)
        {
            var allocation = new Allocation
            {
                Id = 0,
                AnalyticId = allocationDto.AnalyticId,
                StartDate = month.Date.Date,
                Percentage = month.Percentage.GetValueOrDefault(),
                EmployeeId = allocationDto.EmployeeId,
                ReleaseDate = allocationDto.ReleaseDate.GetValueOrDefault().Date
            };

            unitOfWork.AllocationRepository.Insert(allocation);
        }
    }
}
