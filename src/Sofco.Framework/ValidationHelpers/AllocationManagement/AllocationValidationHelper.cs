using System;
using System.Collections.Generic;
using Sofco.Model.Models.TimeManagement;
using Sofco.Model.Utils;
using Sofco.Model.DTO;
using System.Linq;
using Sofco.Model.Enums;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class AllocationValidationHelper
    {
        public static void ValidatePercentage(Response<Allocation> response, decimal? percentage)
        {
            if (!percentage.HasValue || (percentage.GetValueOrDefault() <= 0 || percentage.GetValueOrDefault() > 100))
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.WrongPercentage, MessageType.Error));
            }
        }

        public static void ValidateDates(Response<Allocation> response, DateTime? dateSince, DateTime? dateTo)
        {
            if (!dateSince.HasValue || dateSince.Value < DateTime.MinValue)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.DateSinceRequired, MessageType.Error));
            }

            if (!dateTo.HasValue || dateTo.Value < DateTime.MinValue)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.DateToRequired, MessageType.Error));
            }

            if (!response.HasErrors() && dateTo.Value < dateSince.Value)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.DateToLessThanDateSince, MessageType.Error));
            }
        }

        public static void ValidateAnalyticDates(Analytic analytic, Response<Allocation> response, AllocationAsignmentParams parameters)
        {
            if (analytic != null)
            {
                if (parameters.DateSince.Value.Date < analytic.StartDateContract.Date || parameters.DateSince.Value.Date > analytic.EndDateContract.Date)
                {
                    response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.DateSinceOutOfRange, MessageType.Error));
                }

                if (parameters.DateTo.Value.Date < analytic.StartDateContract.Date || parameters.DateTo.Value.Date > analytic.EndDateContract.Date)
                {
                    response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.DateToOutOfRange, MessageType.Error));
                }
            }
        }

        public static void ValidatePercentageRange(Response<Allocation> response, ICollection<Allocation> allocationsBetweenDays, AllocationAsignmentParams parameters)
        {
            var percentageGreaterThan100 = false;

            var datesInit = allocationsBetweenDays.Select(x => x.StartDate).Distinct();
            var datesEnd = allocationsBetweenDays.Select(x => x.EndDate).Distinct();

            var datesTotal = datesInit.Union(datesEnd).Distinct().OrderBy(x => x.Date).ToList();

            for (int i = 1; i < datesTotal.Count(); i++)
            {
                var allocations = allocationsBetweenDays
                    .Where(x => ((x.StartDate >= datesTotal[i-1] && x.StartDate <= datesTotal[i]) ||
                                (x.EndDate >= datesTotal[i - 1] && x.EndDate <= datesTotal[i])));

                if(allocations.Count() > 1)
                {
                    for (DateTime date = datesTotal[i - 1].Date; date.Date <= datesTotal[i].Date; date = date.AddDays(1))
                    {
                        var allocationsPerDay = allocations.Where(x => date.Date >= x.StartDate.Date && date.Date <= x.EndDate.Date);

                        if (allocationsPerDay.Any())
                        {
                            var percentage = allocationsPerDay.Sum(x => x.Percentage);
                            if (percentage + parameters.Percentage > 100) percentageGreaterThan100 = true;
                        }
                    }
                }
                else
                {
                    var percentage = allocations.Sum(x => x.Percentage);
                    if (percentage + parameters.Percentage > 100) percentageGreaterThan100 = true;
                }

            }

            if (percentageGreaterThan100)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.CannotBeAssign, MessageType.Error));
            }
        }
    }
}
