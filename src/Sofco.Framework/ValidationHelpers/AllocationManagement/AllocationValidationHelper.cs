using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.DAL.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.Domain.DTO;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Framework.Helpers;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class AllocationValidationHelper
    {
        public static void ValidatePercentage(Response<Allocation> response, AllocationDto allocation)
        {
            if (allocation.Months.Any(x => x.Percentage < 0 || x.Percentage > 100))
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Allocation.WrongPercentage, MessageType.Error));
            }
        }

        public static void ValidatePercentageRange(Response<Allocation> response, ICollection<Allocation> allocationsBetweenDays, AllocationDto allocationDto)
        {
            var percentageGreaterThan100 = false;

            foreach (var month in allocationDto.Months)
            {
                var allocations = allocationsBetweenDays.Where(x => x.StartDate.Date == month.Date.Date && x.AnalyticId != allocationDto.AnalyticId);

                var sum = allocations.Sum(x => x.Percentage);

                if(sum + month.Percentage > 100)
                {
                    percentageGreaterThan100 = true;
                }
            }

            if (percentageGreaterThan100)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Allocation.CannotBeAssign, MessageType.Error));
            }
        }

        public static void ValidateReleaseDate(Response<Allocation> response, AllocationDto allocation)
        {
            if (allocation.Months.Any(x => x.Percentage.GetValueOrDefault() > 0))
            {
                if (!allocation.ReleaseDate.HasValue || allocation.ReleaseDate == DateTime.MinValue)
                {
                    response.Messages.Add(new Message(Resources.AllocationManagement.Allocation.ReleaseDateIsRequired, MessageType.Error));
                }
            }
        }

        public static void ValidatePercentage(Response response, AllocationMassiveAddModel model)
        {
            if (!model.Percentage.HasValue || model.Percentage < 0 || model.Percentage > 100)
            {
                response.AddError(Resources.AllocationManagement.Allocation.WrongPercentage);
            }
        }

        public static void ValidateDates(Response response, AllocationMassiveAddModel model)
        {
            if (!model.StartDate.HasValue)
            {
                response.AddError(Resources.AllocationManagement.Allocation.DateSinceRequired);
            }

            if (!model.EndDate.HasValue)
            {
                response.AddError(Resources.AllocationManagement.Allocation.DateToRequired);
            }
        }

        public static void ValidateAnalyticClose(Response<Allocation> response, AllocationDto allocation,
            IAnalyticRepository analyticRepository)
        {
            if (analyticRepository.IsClosed(allocation.AnalyticId))
            {
                response.AddError(Resources.AllocationManagement.Analytic.IsClosed);
            }   
        }

        public static void ValidateEmployeeDates(Response<Allocation> response, AllocationDto allocation, IEmployeeRepository employeeRepository)
        {
            var employee = employeeRepository.Get(allocation.EmployeeId);

            foreach (var month in allocation.Months)
            {
                var startDate = new DateTime(employee.StartDate.Year, employee.StartDate.Month, 1);

                if (month.Date.Date < startDate)
                {
                    if (response.Messages.All(x => x.Text != Resources.AllocationManagement.Allocation.DateLessThanEmployeeStartDate))
                    {
                        response.AddError(Resources.AllocationManagement.Allocation.DateLessThanEmployeeStartDate);
                    }
                }

                if (employee.EndDate.HasValue)
                {
                    var endDate = new DateTime(employee.EndDate.Value.Year, employee.EndDate.Value.Month, 1);

                    if (employee.EndDate.Value > endDate.Date)
                    {
                        if (response.Messages.All(x => x.Text != Resources.AllocationManagement.Allocation.DateGreaterThanEmployeeEndDate))
                        {
                            response.AddError(Resources.AllocationManagement.Allocation.DateGreaterThanEmployeeEndDate);
                        }
                    }
                }
            }
        }

        public static void ValidateWorkTimes(Response<Allocation> response, AllocationDto allocation, IWorkTimeRepository workTimeRepository)
        {
            foreach (var month in allocation.Months)
            {
                if (month.Percentage == 0 && workTimeRepository.EmployeeHasHoursInDate(month.Date, allocation.EmployeeId))
                {
                    response.AddErrorAndNoTraslate($"El mes {DatesHelper.GetDateDescription(month.Date)} no puede tener asignación 0 porque ya contiene horas cargadas");
                }
            }
        }
    }
}
