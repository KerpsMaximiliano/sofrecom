using System;
using System.Collections;
using System.Collections.Generic;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Framework.Helpers;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AllocationService : IAllocationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<AllocationService> logger;
        private readonly IAllocationFileManager allocationFileManager;

        public AllocationService(IUnitOfWork unitOfWork, ILogMailer<AllocationService> logger, IAllocationFileManager allocationFileManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.allocationFileManager = allocationFileManager;
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

            return response;
        }

        public AllocationResponse GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate)
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

            foreach (var analyticId in analyticsIds)
            {
                var allocationDto = new AllocationDto();
                allocationDto.EmployeeId = employeeId;

                var allocation = allocations.Where(x => x.AnalyticId == analyticId).ToList();

                if (allocation.All(x => x.Percentage == 0)) continue;

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
                    allocationDto.AnalyticTitle = string.Empty;
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

                    if (!allocationDto.ReleaseDate.HasValue)
                    {
                        allocationDto.ReleaseDate = allocationMonth?.ReleaseDate.Date == DateTime.MinValue ? null : allocationMonth?.ReleaseDate.Date;
                    }

                    allocationDto.Months.Add(allocationMonthDto);
                }

                allocationResponse.Allocations.Add(allocationDto);
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
                monthHeader.EmployeeHasLicense = licenses.Any(x => x.StartDate.Month == date.Month || x.EndDate.Month == date.Month);

                allocationResponse.MonthsHeader.Add(monthHeader);
            }
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
                            var percentageSum = allocationsFiltered.Sum(x => x.Percentage);

                            if (percentageSum + model.Percentage > 100)
                            {
                                var employee = allocationsFiltered.FirstOrDefault()?.Employee;

                                employeesWithError.Add(new Tuple<string, string, decimal>($"{employee?.EmployeeNumber} - {employee?.Name}", firstMonthAux.Date.ToString("Y"), percentageSum));
                            }
                            else
                            {
                                InsertNewAllocation(model, employeeId, firstMonthAux);
                            }
                        }
                        else
                        {
                            InsertNewAllocation(model, employeeId, firstMonthAux);
                        }

                        firstMonthAux = firstMonthAux.AddMonths(1);
                    }
                }

                unitOfWork.Save();

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
            parameters.StartDate = new DateTime(parameters.StartDate.Year, parameters.StartDate.Month, 1);
            parameters.EndDate = new DateTime(parameters.EndDate.Year, parameters.EndDate.Month, DateTime.DaysInMonth(parameters.EndDate.Year, parameters.EndDate.Month));

            var employees = unitOfWork.AllocationRepository.GetByEmployeesForReport(parameters);

            var response = new Response<AllocationReportModel> { Data = new AllocationReportModel() };

            IList<Employee> employeesUnassigned = new List<Employee>();

            var diccionaryAnalytics = new Dictionary<int, Analytic>();

            if (parameters.StartPercentage.GetValueOrDefault() == 0 &&
                parameters.EndPercentage.GetValueOrDefault() == 0)
            {
                employeesUnassigned = unitOfWork.EmployeeRepository.GetUnassigned();
            }

            if (employees.Any() || employeesUnassigned.Any())
            {
                foreach (var employee in employees)
                {
                    var allocationResponse = GetAllocationsBetweenDays(employee.Id, parameters.StartDate, parameters.EndDate);

                    foreach (var allocation in allocationResponse.Allocations)
                    {

                        if (parameters.AnalyticIds.Any() && parameters.AnalyticIds.All(x => x != allocation.AnalyticId)) continue;
                        //if (parameters.Percentage.HasValue && parameters.Percentage != (int)AllocationPercentage.Differente100 && allocation.Months.All(x => x.Percentage != parameters.Percentage)) continue;
                        //if (parameters.Percentage.HasValue && parameters.Percentage == (int)AllocationPercentage.Differente100 && allocation.Months.All(x => x.Percentage == 100)) continue;

                        if (parameters.Percentage.HasValue && parameters.Percentage > 0 &&
                            !allocation.Months.Any(x => x.Percentage >= parameters.StartPercentage.GetValueOrDefault() &&
                                                       x.Percentage <= parameters.EndPercentage.GetValueOrDefault())) continue;

                        Analytic analytic;

                        if (diccionaryAnalytics.ContainsKey(allocation.AnalyticId))
                        {
                            diccionaryAnalytics.TryGetValue(allocation.AnalyticId, out analytic);
                        }
                        else
                        {
                            analytic = unitOfWork.AnalyticRepository.GetById(allocation.AnalyticId);
                            diccionaryAnalytics.Add(allocation.AnalyticId, analytic);
                        }

                        var reportRow = new AllocationReportRow
                        {
                            Manager = employee.Manager?.Name,
                            Percentage = employee.BillingPercentage,
                            Profile = employee.Profile,
                            ResourceName = $"{employee.EmployeeNumber} - {employee.Name}",
                            Seniority = employee.Seniority,
                            Technology = employee.Technology,
                            Analytic = $"{allocation.AnalyticTitle} - {analytic?.Name}"
                        };

                        for (int i = 0; i < allocation.Months.Count; i++)
                        {
                            reportRow.Months.Add(new AllocationMonthReport { MonthYear = allocationResponse.MonthsHeader[i].Display, Percentage = allocation.Months[i].Percentage.GetValueOrDefault() });
                        }

                        response.Data.Rows.Add(reportRow);
                    }
                }

                foreach (var employee in employeesUnassigned)
                {
                    var licenses = unitOfWork.LicenseRepository.GetByEmployeeAndDates(employee.Id, parameters.StartDate, parameters.EndDate);

                    var allocationResponse = new AllocationResponse();

                    BuildMonthHeader(parameters.StartDate, parameters.EndDate, licenses, allocationResponse);

                    if (licenses.Any())
                    {
                        allocationResponse.Licenses = licenses.Select(x => new LicenseItemDto(x)).ToList();
                    }

                    var reportRow = new AllocationReportRow
                    {
                        Manager = employee.Manager?.Name,
                        Percentage = employee.BillingPercentage,
                        Profile = employee.Profile,
                        ResourceName = $"{employee.EmployeeNumber} - {employee.Name}",
                        Seniority = employee.Seniority,
                        Technology = employee.Technology,
                        Analytic = "Sin Asignación"
                    };

                    foreach (var monthHeader in allocationResponse.MonthsHeader)
                    {
                        reportRow.Months.Add(new AllocationMonthReport { MonthYear = monthHeader.Display, Percentage = 0 });
                    }

                    response.Data.Rows.Add(reportRow);
                }

                response.Data.MonthsHeader = response.Data.Rows.FirstOrDefault()?.Months.Select(x => x.MonthYear).ToList();
            }
            else
            {
                response.AddWarning(Resources.AllocationManagement.Employee.EmployeesNotFound);
            }

            return response;
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
