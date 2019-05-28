using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.ManagementReport
{
    public class ManagementReportCalculateCostsService : IManagementReportCalculateCostsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ManagementReportCalculateCostsService> logger;

        public ManagementReportCalculateCostsService(IUnitOfWork unitOfWork,
            ILogMailer<ManagementReportCalculateCostsService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public void CalculateCosts(AllocationDto allocationDto, DateTime firstMonthDate, DateTime lastMonthDate)
        {
            try
            {
                Calculate(allocationDto, firstMonthDate, lastMonthDate);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        public void UpdateManagementReports(Response response, int year, int month)
        {
            try
            {
                var date = new DateTime(year, month, 1);

                var costDetailResources = unitOfWork.CostDetailResourceRepository.GetByDate(date);
                var allocations = unitOfWork.AllocationRepository.GetAllocationsByDate(date);

                foreach (var costDetailResource in costDetailResources)
                {
                    var allocation = allocations.SingleOrDefault(x => x.AnalyticId == costDetailResource.CostDetail.ManagementReport.AnalyticId && x.EmployeeId == costDetailResource.EmployeeId);

                    if (allocation != null)
                    {
                        costDetailResource.Value = (allocation.Percentage / 100) * allocation.Employee.Salary;
                        costDetailResource.Charges = (allocation.Percentage / 100) * allocation.Employee.PrepaidAmount;
                        unitOfWork.CostDetailResourceRepository.Update(costDetailResource);
                    }
                }

                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddWarning(Resources.ManagementReport.ManagementReport.UpdatePrepaidError);
            }
        }

        private void Calculate(AllocationDto allocationDto, DateTime firstMonthDate, DateTime lastMonthDate)
        {
            var allocationsBetweenDays = unitOfWork.AllocationRepository.GetAllocationsBetweenDays(allocationDto.EmployeeId, firstMonthDate.Date, lastMonthDate.Date);

            var analyticIds = allocationsBetweenDays.Select(x => x.AnalyticId).Distinct();
            var allocationFirst = allocationsBetweenDays.FirstOrDefault();

            if (allocationFirst == null)
            {
                var managementReport = unitOfWork.ManagementReportRepository.GetSingle(x => x.AnalyticId == allocationDto.AnalyticId);

                for (DateTime date = firstMonthDate; date <= lastMonthDate; date = date.AddMonths(1))
                {
                    var costDetail = unitOfWork.CostDetailRepository.GetWithResourceDetails(managementReport.Id, date);

                    if(costDetail == null) continue;

                    var resource = costDetail.CostDetailResources.SingleOrDefault(x => x.EmployeeId == allocationDto.EmployeeId);

                    if(resource == null) continue;

                    costDetail.CostDetailResources.Remove(resource);
                    unitOfWork.CostDetailRepository.Update(costDetail);
                }

                unitOfWork.Save();

                return;
            }

            var employee = unitOfWork.EmployeeRepository.Get(allocationFirst.EmployeeId);
            var user = unitOfWork.UserRepository.GetByEmail(employee.Email);

            var diccionary = new Dictionary<int, int>();

            foreach (var analyticId in analyticIds)
            {
                var managementReport = unitOfWork.ManagementReportRepository.GetSingle(x => x.AnalyticId == analyticId);

                if (managementReport == null) continue;

                diccionary.Add(analyticId, managementReport.Id);
            }

            for (DateTime date = firstMonthDate; date <= lastMonthDate; date = date.AddMonths(1))
            {
                var allocations = allocationsBetweenDays.Where(x => x.StartDate.Date == date.Date).ToList();

                foreach (var allocation in allocations)
                {
                    var managementReportId = diccionary[allocation.AnalyticId];

                    var costDetail = unitOfWork.CostDetailRepository.GetWithResourceDetails(managementReportId, date);

                    var resource = costDetail.CostDetailResources.SingleOrDefault(x => x.EmployeeId == allocation.EmployeeId);

                    if (resource == null)
                    {
                        costDetail.CostDetailResources.Add(new CostDetailResource
                        {
                            Charges = (allocation.Percentage / 100) * employee.PrepaidAmount,
                            EmployeeId = employee.Id,
                            Value = (allocation.Percentage / 100) * employee.Salary,
                            UserId = user.Id
                        });

                        unitOfWork.CostDetailRepository.Update(costDetail);
                    }
                    else
                    {
                        var allocationPercentage = allocation.Percentage == 100 ? 1 : allocation.Percentage / 100;

                        resource.Charges = allocationPercentage * employee.PrepaidAmount;
                        resource.Value = allocationPercentage * employee.Salary;

                        unitOfWork.CostDetailResourceRepository.Update(resource);
                    }
                }
            }

            unitOfWork.Save();
        }
    }
}
