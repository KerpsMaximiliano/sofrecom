using System;
using System.Collections.Generic;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Framework.Helpers;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AllocationService : IAllocationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<AllocationService> logger;

        public AllocationService(IUnitOfWork unitOfWork, ILogMailer<AllocationService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
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

            //Build Header
            for (DateTime date = startDate.Date; date.Date <= endDate.Date; date = date.AddMonths(1))
            {
                var monthHeader = new MonthHeader();
                monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                monthHeader.Month = date.Month;
                monthHeader.Year = date.Year;
                monthHeader.EmployeeHasLicense = licenses.Any(x => x.StartDate.Month == date.Month || x.EndDate.Month == date.Month);

                allocationResponse.MonthsHeader.Add(monthHeader);
            }

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
                    allocationMonthDto.ReleaseDate = allocationMonth?.ReleaseDate.Date ?? DateTime.UtcNow;

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

        public ICollection<Employee> GetByService(string serviceId)
        {
            return unitOfWork.AllocationRepository.GetByService(serviceId);
        }

        public ICollection<Employee> GetByEmployeesByAnalytic(int analyticId)
        {
            return unitOfWork.AllocationRepository.GetByAnalyticId(analyticId);
        }

        public Response<AllocationReportModel> CreateReport(AllocationReportParams parameters)
        {
            parameters.StartDate = new DateTime(parameters.StartDate.Year, parameters.StartDate.Month, 1);
            parameters.EndDate = new DateTime(parameters.EndDate.Year, parameters.EndDate.Month, DateTime.DaysInMonth(parameters.EndDate.Year, parameters.EndDate.Month));

            var employees = unitOfWork.AllocationRepository.GetByEmployeesForReport(parameters);

            var response = new Response<AllocationReportModel> { Data = new AllocationReportModel() };

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    var allocationResponse = GetAllocationsBetweenDays(employee.Id, parameters.StartDate, parameters.EndDate);

                    foreach (var allocation in allocationResponse.Allocations)
                    {

                        if (parameters.AnalyticId.HasValue && parameters.AnalyticId != allocation.AnalyticId) continue;
                        //if (parameters.Percentage.HasValue && parameters.Percentage != (int)AllocationPercentage.Differente100 && allocation.Months.All(x => x.Percentage != parameters.Percentage)) continue;
                        //if (parameters.Percentage.HasValue && parameters.Percentage == (int)AllocationPercentage.Differente100 && allocation.Months.All(x => x.Percentage == 100)) continue;

                        if (parameters.Percentage.HasValue && parameters.Percentage > 0 &&
                            !allocation.Months.Any(x => x.Percentage >= parameters.StartPercentage.GetValueOrDefault() &&
                                                       x.Percentage <= parameters.EndPercentage.GetValueOrDefault())) continue;

                        var analytic = unitOfWork.AnalyticRepository.GetById(allocation.AnalyticId);

                        var reportRow = new AllocationReportRow();

                        reportRow.Manager = analytic.Manager?.Name;
                        reportRow.Percentage = employee.BillingPercentage;
                        reportRow.Profile = employee.Profile;
                        reportRow.ResourceName = employee.Name;
                        reportRow.Seniority = employee.Seniority;
                        reportRow.Technology = employee.Technology;
                        reportRow.Analytic = allocation.AnalyticTitle;

                        for (int i = 0; i < allocation.Months.Count; i++)
                        {
                            reportRow.Months.Add(new AllocationMonthReport { MonthYear = allocationResponse.MonthsHeader[i].Display, Percentage = allocation.Months[i].Percentage.GetValueOrDefault() });
                        }

                        response.Data.Rows.Add(reportRow);
                    }
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
                        InsertNewAllocation(allocationDto, month);
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
            Allocation allocation = new Allocation
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
