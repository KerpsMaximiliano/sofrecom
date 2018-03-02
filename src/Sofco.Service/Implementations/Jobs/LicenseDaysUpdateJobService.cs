using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Model.Models.AllocationManagement;

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
            var employees = unitOfWork.EmployeeRepository.GetAll();
            GetHolidaysValues();

            try
            {
                foreach (var employee in employees)
                {
                    var daysWorked = CalculateDaysWorked(employee);

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
                if (employeeHistory.EndDate.HasValue && employeeHistory.StartDate.HasValue &&
                    employeeHistory.EndDate.Value.Date > employeeHistory.StartDate.Value.Date)
                {
                    days += DateTime.UtcNow.Date.Subtract(employee.StartDate.Date).Days + 1;
                }
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

            var daysAvg = (double)daysWorked / 20;

            if (daysAvg % 1 >= 0.5)
            {
                daysAvg++;
            }

            var days = (int) daysAvg;

            if (days > 6)
            {
                days -= 2;
            }
            else if (days == 6)
            {
                days--;
            }

            employee.HolidaysPending = days;
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
            var settings = unitOfWork.GlobalSettingRepository.GetHolidaysValues();

            foreach (var globalSetting in settings)
            {
                holidaysValues.Add(globalSetting.Key, Convert.ToInt32(globalSetting.Value));
            }
        }

        private void CalculateDaysPending(Employee employee, string key)
        {
            employee.HolidaysPending += holidaysValues[key];

            if (employee.HasExtraHolidays)
            {
                employee.HolidaysPending += employee.ExtraHolidaysQuantity;
            }
        }

        private int CalculateDaysWorked(Employee employee)
        {
            var days = new DateTime(DateTime.UtcNow.Year, 12, 31).Subtract(employee.StartDate.Date).Days + 1;

            return days;
        }
    }
}
