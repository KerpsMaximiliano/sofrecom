using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

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

                this.InsertMissingEmployees(date);

                var costDetailResources = unitOfWork.CostDetailResourceRepository.GetByDate(date);
                var allocations = unitOfWork.AllocationRepository.GetAllocationsByDate(date);

                foreach (var costDetailResource in costDetailResources)
                {
                    var allocation = allocations.SingleOrDefault(x => x.AnalyticId == costDetailResource.CostDetail.ManagementReport.AnalyticId && x.EmployeeId == costDetailResource.EmployeeId);

                    if (allocation != null && allocation.Percentage > 0)
                    {
                        var socialCharge = allocation.Employee.SocialCharges.FirstOrDefault(x => x.Year == year && x.Month == month);

                        if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.SalaryTotal), out var salary)) salary = 0;
                        if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.ChargesTotal), out var charges)) charges = 0;

                        var newValueSalary = (allocation.Percentage / 100) * salary;
                        var newValueCharges = (allocation.Percentage / 100) * charges;

                        costDetailResource.Value = CryptographyHelper.Encrypt(newValueSalary.ToString(CultureInfo.InvariantCulture));
                        costDetailResource.Charges = CryptographyHelper.Encrypt(newValueCharges.ToString(CultureInfo.InvariantCulture));
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

        private void InsertMissingEmployees(DateTime date)
        {
            try
            {
                var reports = unitOfWork.ManagementReportRepository.GetByDate(date);

                foreach (var report in reports)
                {
                    var managementReport = unitOfWork.ManagementReportRepository.GetById(report.Id);
                    var costDetails = managementReport.CostDetails;

                    var IdEmployees = costDetails
                                       .SelectMany(x => x.CostDetailResources.Select(d => d.EmployeeId))
                                       .Distinct()
                                       .ToArray();

                    int costDetailId = costDetails.Where(x => x.MonthYear.Date == date.Date).Select(x => x.Id).FirstOrDefault();
                    var costDetailResources = costDetails.Where(x => x.MonthYear.Date == date.Date).Select(x => x.CostDetailResources).FirstOrDefault();

                    foreach (var employeeId in IdEmployees)
                    {
                        if (!costDetailResources.Any(x => x.EmployeeId == employeeId))
                        {
                            var entity = new CostDetailResource();

                            entity.CostDetailId = costDetailId;
                            entity.EmployeeId = employeeId;

                            unitOfWork.CostDetailResourceRepository.Insert(entity);
                        }
                    }
                }

                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                throw e;
            }
        }

        private void Calculate(AllocationDto allocationDto, DateTime firstMonthDate, DateTime lastMonthDate)
        {
            var allocationsBetweenDays = unitOfWork.AllocationRepository.GetAllocationsBetweenDaysWithCharges(allocationDto.EmployeeId, firstMonthDate.Date, lastMonthDate.Date);

            var analyticIds = allocationsBetweenDays.Select(x => x.AnalyticId).Distinct();
            var allocationFirst = allocationsBetweenDays.FirstOrDefault();

            if (allocationFirst == null)
            {
                var managementReport = unitOfWork.ManagementReportRepository.GetSingle(x => x.AnalyticId == allocationDto.AnalyticId);

                for (DateTime date = firstMonthDate; date <= lastMonthDate; date = date.AddMonths(1))
                {
                    var costDetail = unitOfWork.CostDetailRepository.GetWithResourceDetails(managementReport.Id, date);

                    if (costDetail == null) continue;

                    var resource = costDetail.CostDetailResources.SingleOrDefault(x => x.EmployeeId == allocationDto.EmployeeId);

                    if (resource == null) continue;

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

                    var newCostDetail = false;

                    if (costDetail == null)
                    {
                        newCostDetail = true;
                        costDetail = new CostDetail
                        {
                            ManagementReportId = managementReportId,
                            ContratedDetails = new List<ContratedDetail>(),
                            CostDetailOthers = new List<CostDetailOther>(),
                            CostDetailResources = new List<CostDetailResource>(),
                            CostDetailProfiles = new List<CostDetailProfile>(),
                            MonthYear = date,
                        };
                    }

                    var resource = costDetail.CostDetailResources.SingleOrDefault(x => x.EmployeeId == allocation.EmployeeId);

                    var socialCharge = allocation.Employee.SocialCharges.FirstOrDefault(x => x.Year == allocation.StartDate.Year && x.Month == allocation.StartDate.Month);

                    if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.SalaryTotal), out var salary)) salary = 0;
                    if (!decimal.TryParse(CryptographyHelper.Decrypt(socialCharge?.ChargesTotal), out var charges)) charges = 0;

                    if (resource == null)
                    { 
                        var newValueSalary = (allocation.Percentage / 100) * salary;
                        var newValueCharges = (allocation.Percentage / 100) * charges;

                        costDetail.CostDetailResources.Add(new CostDetailResource
                        {
                            Charges = CryptographyHelper.Encrypt(newValueCharges.ToString()),
                            EmployeeId = employee.Id,
                            Value = CryptographyHelper.Encrypt(newValueSalary.ToString()),
                            UserId = user.Id
                        });

                        if (newCostDetail)
                            unitOfWork.CostDetailRepository.Insert(costDetail);
                        else
                            unitOfWork.CostDetailRepository.Update(costDetail);

                    }
                    else
                    {
                        var allocationPercentage = allocation.Percentage == 100 ? 1 : allocation.Percentage / 100;

                        var newValueSalary = allocationPercentage * salary;
                        var newValueCharges = (allocation.Percentage / 100) * charges;

                        resource.Charges = CryptographyHelper.Encrypt(newValueCharges.ToString(CultureInfo.InvariantCulture));
                        resource.Value = CryptographyHelper.Encrypt(newValueSalary.ToString(CultureInfo.InvariantCulture));

                        unitOfWork.CostDetailResourceRepository.Update(resource);
                    }
                }
            }

            unitOfWork.Save();
        }
    }
}
