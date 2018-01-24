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

            if(allocationsBetweenDays.Count > 0)
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
                            allocation.Percentage = month.Percentage;
                            unitOfWork.AllocationRepository.UpdatePercentage(allocation);
                        }

                        allocation.ReleaseDate = allocationDto.ReleaseDate.GetValueOrDefault().Date;
                        unitOfWork.AllocationRepository.UpdateReleaseDate(allocation);
                    }
                    else
                    {
                        allocation.AnalyticId = allocationDto.AnalyticId;
                        allocation.StartDate = month.Date.Date;
                        allocation.Percentage = month.Percentage;
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
