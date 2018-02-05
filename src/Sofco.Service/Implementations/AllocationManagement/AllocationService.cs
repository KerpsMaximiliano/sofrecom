﻿using System;
using System.Collections.Generic;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Framework.Helpers;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AllocationService : IAllocationService
    {
        private readonly IUnitOfWork unitOfWork;

        public AllocationService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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

                SaveAllocation(allocation, response);
            }
            else
            {
                SaveAllocation(allocation, response);
            }

            return response;
        }

        public AllocationResponse GetAllocationsBetweenDays(int employeeId, DateTime startDate, DateTime endDate)
        {
            var allocations = unitOfWork.AllocationRepository.GetAllocationsBetweenDays(employeeId, startDate, endDate);

            var allocationResponse = new AllocationResponse();

            //Build Header
            for (DateTime date = startDate.Date; date.Date <= endDate.Date; date = date.AddMonths(1))
            {
                var monthHeader = new MonthHeader();
                monthHeader.Display = DatesHelper.GetDateShortDescription(date);
                monthHeader.Month = date.Month;
                monthHeader.Year = date.Year;

                allocationResponse.MonthsHeader.Add(monthHeader);
            }

            var analyticsIds = allocations.Select(x => x.AnalyticId).Distinct();

            foreach (var analyticId in analyticsIds)
            {
                var allocationDto = new AllocationDto();
                allocationDto.EmployeeId = employeeId;

                var allocation = allocations.Where(x => x.AnalyticId == analyticId);

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
                        allocationDto.ReleaseDate = allocationMonth?.ReleaseDate.Date ?? DateTime.UtcNow;
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

        public Response<AllocationReportModel> CreateReport(AllocationReportParams parameters)
        {
            var employees = unitOfWork.AllocationRepository.GetByEmployeesForReport(parameters);

            var response = new Response<AllocationReportModel> { Data = new AllocationReportModel() };

            parameters.StartDate = new DateTime(parameters.StartDate.Year, parameters.StartDate.Month, 1);
            parameters.EndDate = new DateTime(parameters.EndDate.Year, parameters.EndDate.Month, DateTime.DaysInMonth(parameters.EndDate.Year, parameters.EndDate.Month));

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    var allocationResponse = GetAllocationsBetweenDays(employee.Id, parameters.StartDate, parameters.EndDate);

                    foreach (var allocation in allocationResponse.Allocations)
                    {

                        if (parameters.AnalyticId.HasValue && parameters.AnalyticId != allocation.AnalyticId) continue;
                        if (parameters.Percentage.HasValue && parameters.Percentage != 999 && allocation.Months.All(x => x.Percentage != parameters.Percentage)) continue;
                        if (parameters.Percentage.HasValue && parameters.Percentage == 999 && allocation.Months.All(x => x.Percentage == 100)) continue;

                        var reportRow = new AllocationReportRow();

                        reportRow.Manager = "Diego O. Miguel";
                        reportRow.Office = "Dirección de Soluciones IT";
                        reportRow.Percentage = employee.BillingPercentage;
                        reportRow.ProjectManager = "Juan J. Larenze";
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

        public IList<decimal> GetAllPercentages()
        {
            return unitOfWork.AllocationRepository.GetAllPercentages();
        }

        private void SaveAllocation(AllocationDto allocationDto, Response response)
        {
            try
            {
                foreach (var month in allocationDto.Months)
                {
                    var allocation = new Allocation();

                    if (month.AllocationId > 0)
                    {
                        allocation.Id = month.AllocationId;

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
                        allocation.AnalyticId = allocationDto.AnalyticId;
                        allocation.StartDate = month.Date.Date;
                        allocation.Percentage = month.Percentage.GetValueOrDefault();
                        allocation.EmployeeId = allocationDto.EmployeeId;
                        allocation.ReleaseDate = allocationDto.ReleaseDate.GetValueOrDefault().Date;

                        unitOfWork.AllocationRepository.Insert(allocation);
                    }
                }

                unitOfWork.Save();

                response.Messages.Add(new Message(Resources.AllocationManagement.Allocation.Added, MessageType.Success));
            }
            catch (Exception ex)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }
        }
    }
}
