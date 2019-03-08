using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Service.Implementations.Rrhh
{
    public class LicenseGenerateWorkTimeService : ILicenseGenerateWorkTimeService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly AppSetting appSetting;

        public LicenseGenerateWorkTimeService(IUnitOfWork unitOfWork, IOptions<AppSetting> appSetting)
        {
            this.unitOfWork = unitOfWork;
            this.appSetting = appSetting.Value;
        }
        public void GenerateWorkTimes(License license)
        {
            var startDate = license.StartDate;
            var endDate = license.EndDate;

            var allocationStartDate = new DateTime(startDate.Year, startDate.Month, 1);
            var allocationEndDate = new DateTime(endDate.Year, endDate.Month, 1);

            var allocations = unitOfWork.AllocationRepository.GetAllocationsLiteBetweenDays(license.EmployeeId, allocationStartDate, allocationEndDate);

            var user = unitOfWork.UserRepository.GetByEmail(license.Employee.Email);

            var analyticBank = unitOfWork.AnalyticRepository.GetByTitle(appSetting.AnalyticBank);

            while (startDate.Date <= endDate.Date)
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = startDate.AddDays(1);
                    continue;
                }

                var startDateOfMonth = new DateTime(startDate.Year, startDate.Month, 1);

                var allocationsInMonth = allocations.Where(x => x.StartDate == startDateOfMonth).ToList();

                if (!allocationsInMonth.Any())
                {
                    var worktime = BuildWorkTime(license, startDate, user);

                    worktime.AnalyticId = analyticBank.Id;
                    worktime.Hours = license.Employee.BusinessHours;

                    unitOfWork.WorkTimeRepository.Insert(worktime); 
                }
                else
                {
                    if (allocationsInMonth.All(x => x.Percentage == 0))
                    {
                        var worktime = BuildWorkTime(license, startDate, user);

                        worktime.AnalyticId = analyticBank.Id;
                        worktime.Hours = license.Employee.BusinessHours;

                        unitOfWork.WorkTimeRepository.Insert(worktime);
                    }
                    else
                    {
                        decimal hours = 0;

                        foreach (var allocation in allocationsInMonth)
                        {
                            if (allocation.Percentage > 0)
                            {
                                var worktime = BuildWorkTime(license, startDate, user);

                                worktime.AnalyticId = allocation.AnalyticId;

                                worktime.Hours = (license.Employee.BusinessHours * allocation.Percentage) / 100;
                                hours += worktime.Hours;

                                unitOfWork.WorkTimeRepository.Insert(worktime);
                            }
                        }

                        if (hours < license.Employee.BusinessHours)
                        {
                            var diff = license.Employee.BusinessHours - hours;

                            var worktime = BuildWorkTime(license, startDate, user);

                            worktime.AnalyticId = analyticBank.Id;
                            worktime.Hours = diff;

                            unitOfWork.WorkTimeRepository.Insert(worktime);
                        }
                    }
                }

                startDate = startDate.AddDays(1);
            }

            unitOfWork.Save();
        }

        private WorkTime BuildWorkTime(License license, DateTime startDate, Domain.Models.Admin.User user)
        {
            var worktime = new WorkTime
            {
                EmployeeId = license.EmployeeId,
                UserId = user.Id,
                UserComment = license.Type.Description,
                CreationDate = DateTime.UtcNow.Date,
                Status = WorkTimeStatus.License,
                Date = startDate.Date,
                TaskId = license.Type.TaskId,
                Source = WorkTimeSource.License.ToString()
            };

            return worktime;
        }
    }
}
