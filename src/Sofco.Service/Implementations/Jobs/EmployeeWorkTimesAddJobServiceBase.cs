using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.Helpers;
using Sofco.Service.Implementations.Entities;

namespace Sofco.Service.Implementations.Jobs
{
    public abstract class EmployeeWorkTimesAddJobServiceBase 
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<EmployeeWorkTimesAddJobService> logger;

        #region private properties
        private int? taskID;
        private int TaskID
        {
            get
            {
                if (taskID == null)
                    taskID = GetTaskID();

                return taskID.Value;
            }

        }
        #endregion

        public EmployeeWorkTimesAddJobServiceBase(IUnitOfWork unitOfWork, ILogMailer<EmployeeWorkTimesAddJobService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public void Run()
        {
            if (!this.MustExecute())
                return;

            List<DateTime> days = GetDaysToAnalize();

            foreach (DateTime day in days)
            {
                try
                {
                    IList<Allocation> allocations = GetAllocations(day);
                    IList<RescheduledAllocation> rescheduledAllocations = UpdateAllocationsByWorkTimes(allocations, day);

                    foreach (RescheduledAllocation rescheduledAllocation in rescheduledAllocations)
                    {
                        WorkTime workTime = CreateDomain(rescheduledAllocation, day);
                        unitOfWork.WorkTimeRepository.Save(workTime);
                    }


                    unitOfWork.Save();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex);
                }
            }
        }

        protected abstract bool MustExecute();

        private IList<RescheduledAllocation> UpdateAllocationsByWorkTimes(IList<Allocation> allocations, DateTime date)
        {
            List<RescheduledAllocation> rescheduledAllocations = new List<RescheduledAllocation>();

            List<Employee> employees = allocations.Select(x => x.Employee).Distinct().ToList();
            foreach (Employee employee in employees)
            {
                var workTimes = this.unitOfWork.WorkTimeRepository.GetLiteByEmployeeId(date, employee.Id);
                decimal employeWorkedHours = workTimes.Sum(x => x.Hours);

                if (employee.EndDate.HasValue && employee.EndDate.Value.Date < date.Date)
                    continue;
                if (employeWorkedHours >= employee.BusinessHours || workTimes.Any(x => x.Source == WorkTimeSource.License.ToString()))
                    continue;

                var filteredAllocations = allocations.Where(x => x.EmployeeId == employee.Id 
                    && !workTimes.Any(p => p.AnalyticId == x.AnalyticId) 
                    && x.Analytic.AutomaticChargeable.HasValue && x.Analytic.AutomaticChargeable.Value).ToList();
                var totalPorcentaje = filteredAllocations.Sum(allocation => allocation.Percentage);

                foreach (Allocation employeeAllocation in filteredAllocations)
                {
                    RescheduledAllocation rescheduledAllocation = new RescheduledAllocation();
                    rescheduledAllocation.AnalyticId = employeeAllocation.AnalyticId;
                    rescheduledAllocation.EmployeeId= employeeAllocation.EmployeeId;
                    rescheduledAllocation.EmployeeEmail = employee.Email;
                    rescheduledAllocation.Date = date;
                    rescheduledAllocation.RemainBusinessHours = employee.BusinessHours - employeWorkedHours;
                    rescheduledAllocation.Percentage = employeeAllocation.Percentage * 100 / totalPorcentaje;
                    //if (employeeAllocation.Analytic.AutomaticChargeable.HasValue && employeeAllocation.Analytic.AutomaticChargeable.Value)
                    rescheduledAllocations.Add(rescheduledAllocation);
                }
            }

            return rescheduledAllocations;
        }

        public WorkTime CreateDomain(RescheduledAllocation allocation, DateTime day)
        {
            var domain = new WorkTime();

            domain.AnalyticId = allocation.AnalyticId;
            domain.EmployeeId = allocation.EmployeeId;
            domain.UserId = this.unitOfWork.UserRepository.GetByEmail(allocation.EmployeeEmail).Id;
            domain.TaskId = TaskID;
            domain.Date = allocation.Date;
            domain.Hours = CalculateHours(allocation);
            domain.Source = WorkTimeSource.AutomaticProcess.ToString();

            domain.CreationDate = DateTime.UtcNow.Date;
            domain.Status = WorkTimeStatus.Approved;

            return domain;
        }

        private  decimal CalculateHours(RescheduledAllocation allocation)
        {
            List<WorkTime> workTimesInMemory = unitOfWork.WorkTimeRepository.GetAddTrackedByEmployee(allocation.EmployeeId);
            Decimal maxHours = allocation.RemainBusinessHours - workTimesInMemory.Sum(x => x.Hours);
            Decimal calculateHours = allocation.RemainBusinessHours * allocation.Percentage / 100;
            calculateHours = Math.Round(calculateHours, 2);

            if (calculateHours >= maxHours)
                return maxHours;
            if (calculateHours + (allocation.RemainBusinessHours / 100) > maxHours)
                return maxHours;

            return calculateHours;
        }

        private IList<Allocation> GetAllocations(DateTime day)
        {
            IList<Allocation> allocations = this.unitOfWork.AllocationRepository.GetAllocationsLiteBetweenDatesAndFullBillingPercentage(day);

            // Filtrar registros  por combinación de id_empleado e id_analitic tomando  el que tiene startDate mas cercano de cada uno
            return allocations.GroupBy(a => new { a.EmployeeId, a.AnalyticId })
                .Select(group => group.OrderBy(a => Math.Abs((day - a.StartDate).TotalDays)).FirstOrDefault())
                .ToList();
        }

        protected virtual List<DateTime> GetDaysToAnalize()
        {
            List<DateTime> days = GetDaysUntilFirstPreviousMonday(DateTime.Now);
            return this.FilterDaysHolidays(days);
        }

        protected List<DateTime> FilterDaysHolidays(List<DateTime> days)
        {
            List<Holiday> holidays = this.unitOfWork.HolidayRepository.Get(DateTime.Now.Year);
            return days.FindAll(d => !IsHoliday(d, holidays));
        }

        private bool IsHoliday(DateTime d, List<Holiday> holidays)
        {
            return holidays.FirstOrDefault(h => h.Date == d.Date) != null;
        }
        private List<DateTime> GetDaysUntilFirstPreviousMonday(DateTime date)
        {
            List<DateTime> days = new List<DateTime>();

            date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);

            while (date.DayOfWeek != DayOfWeek.Monday)
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    
                    days.Add(date);
                }
                date = date.AddDays(-1);
            }

            days.Add(date);
            days.Reverse();
            return days;
        }

        private int GetTaskID()
        {
            var setting = unitOfWork.SettingRepository.GetByKey(SettingConstant.PreLoadHoursDefault);
            return Convert.ToInt32(setting.Value);
           
        }

    }
}
