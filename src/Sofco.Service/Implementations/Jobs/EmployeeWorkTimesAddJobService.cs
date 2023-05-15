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
    public class EmployeeWorkTimesAddJobService : IEmployeeWorkTimesAddJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly Dictionary<string, int> holidaysValues;
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

        public EmployeeWorkTimesAddJobService(IUnitOfWork unitOfWork, ILogMailer<EmployeeWorkTimesAddJobService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.holidaysValues = new Dictionary<string, int>();
            this.logger = logger;
        }

        public void Run()
        {
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

        private IList<RescheduledAllocation> UpdateAllocationsByWorkTimes(IList<Allocation> allocations, DateTime date)
        {
            List<RescheduledAllocation> rescheduledAllocations = new List<RescheduledAllocation>();

            List<Employee> employees = allocations.Select(x => x.Employee).Distinct().ToList();
            foreach (Employee employee in employees) 
            {
                var workTimes = this.unitOfWork.WorkTimeRepository.GetLiteByEmployeeId(date, employee.Id);
                decimal employeWorkedHours = workTimes.Sum(x => x.Hours);

                if (employeWorkedHours >= employee.BusinessHours || workTimes.Any(x => x.Source == WorkTimeSource.License.ToString()))
                    continue;

                var filteredAllocations = allocations.Where(x => x.EmployeeId == employee.Id && !workTimes.Any(p => p.AnalyticId == x.AnalyticId)).ToList();
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
            domain.Hours = allocation.RemainBusinessHours * allocation.Percentage/100;
            domain.Source = WorkTimeSource.MassiveImport.ToString();

            domain.CreationDate = DateTime.UtcNow.Date;
            domain.Status = WorkTimeStatus.Approved;

            return domain;
        }
        private IList<Allocation> GetAllocations(DateTime day)
        {
            IList<Allocation> allocations = this.unitOfWork.AllocationRepository.GetAllocationsLiteBetweenDatesAndFullBillingPercentage(day);

            // Filtrar registros  por combinación de id_empleado e id_analitic tomando  el que tiene startDate mas cercano de cada uno
            return allocations.GroupBy(a => new { a.EmployeeId, a.AnalyticId })
                .Select(group => group.OrderBy(a => Math.Abs((day - a.StartDate).TotalDays)).FirstOrDefault())
                .ToList();
        }

        private List<DateTime> GetDaysToAnalize()
        {
            List<DateTime> days = GetDaysUntilFirstPreviousMonday(DateTime.Now);
            return this.FilterDaysHolidays(days);
        }

        private List<DateTime> FilterDaysHolidays(List<DateTime> days)
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
