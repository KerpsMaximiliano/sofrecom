using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Framework.Helpers;

namespace Sofco.Service.Implementations.Jobs
{
    public class LicenseDaysUpdateJobService : ILicenseDaysUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly Dictionary<string, int> holidaysValues;
        private readonly ILogMailer<LicenseDaysUpdateJobService> logger;

        public LicenseDaysUpdateJobService(IUnitOfWork unitOfWork, ILogMailer<LicenseDaysUpdateJobService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.holidaysValues = new Dictionary<string, int>();
            this.logger = logger;
        }

        public void Run()
        {
            var employees = unitOfWork.EmployeeRepository.GetAll().ToList();
            
            GetHolidaysValues();

            try
            {
                foreach (var employee in employees)
                {
                    var daysWorked = DatesHelper.GetWorkedDays(employee);

                    if (daysWorked < 181)
                    {
                        CalculateDaysAfterJune30(employee, daysWorked);
                    }
                    else
                    {
                        var historyDays = GetDaysHistory(employee);
                        daysWorked += historyDays;

                        CalculateNormalHolidays(employee, daysWorked);
                    }

                    unitOfWork.EmployeeRepository.Update(employee);
                }

                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private int GetDaysHistory(Employee employee)
        {
            var history = unitOfWork.EmployeeHistoryRepository.GetByEmployeeNumber(employee.EmployeeNumber);

            var days = 0;

            foreach (var employeeHistory in history)
            {
                var startDate = employeeHistory.StartDate.Value;
                var endDate = employeeHistory.EndDate ?? employee.StartDate.AddDays(1);
                days += endDate.Subtract(startDate).Days + 1;
            }

            return days;
        }

        private void CalculateDaysAfterJune30(Employee employee, int daysWorked)
        {
            var licenses = unitOfWork.LicenseRepository.GetByEmployee(employee.Id);

            if (licenses.Any())
            {
                daysWorked -= licenses.Where(x => !x.WithPayment).Sum(x => x.DaysQuantity);
            }

            DatesHelper.SetHolydayDays(employee, daysWorked);
        }

        private void CalculateNormalHolidays(Employee employee, int daysWorked)
        {
            if (daysWorked > 180 && (daysWorked / 365) < 5)
            {
                CalculateDaysPending(employee, "Holidays10Days");
            }

            if ((daysWorked / 365) >= 5 && (daysWorked / 365) < 10)
            {
                CalculateDaysPending(employee, "Holidays15Days");
            }

            if ((daysWorked / 365) >= 10 && (daysWorked / 365) < 20)
            {
                CalculateDaysPending(employee, "Holidays20Days");
            }

            if ((daysWorked / 365) >= 20)
            {
                CalculateDaysPending(employee, "Holidays25Days");
            }
        }

        private void GetHolidaysValues()
        {
            var settings = unitOfWork.SettingRepository.GetHolidaysValues();

            foreach (var globalSetting in settings)
            {
                if (globalSetting.Type == SettingValueType.Number)
                {
                    holidaysValues.Add(globalSetting.Key, Convert.ToInt32(globalSetting.Value));
                }
            }
        }

        private void CalculateDaysPending(Employee employee, string key) 
        {
            employee.HolidaysPending += GetBusinessHolidyas(holidaysValues[key]);
            employee.HolidaysByLaw = holidaysValues[key];
            employee.HolidaysPendingByLaw += holidaysValues[key];

            if (employee.HasExtraHolidays)
            {
                employee.HolidaysPending += employee.ExtraHolidaysQuantity;
                employee.HolidaysPendingByLaw += employee.ExtraHolidaysQuantityByLaw;
            }
        }

        private int GetBusinessHolidyas(int days)
        {
            if (days >= 1 && days <= 7) return 5;
            if (days >= 8 && days <= 14) return 10;
            if (days >= 15 && days <= 21) return 15;
            if (days >= 22 && days <= 28) return 20;
            if (days >= 29 && days <= 35) return 25;

            return 0;
        }
    }
}
