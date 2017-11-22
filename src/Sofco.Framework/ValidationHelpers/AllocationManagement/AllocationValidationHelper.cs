using System;
using System.Collections.Generic;
using Sofco.Model.Utils;
using Sofco.Model.DTO;
using System.Linq;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Framework.ValidationHelpers.AllocationManagement
{
    public static class AllocationValidationHelper
    {
        public static void ValidatePercentage(Response<Allocation> response, AllocationDto allocation)
        {
            if (allocation.Months.Any(x => x.Percentage < 0 || x.Percentage > 100))
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.WrongPercentage, MessageType.Error));
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
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.CannotBeAssign, MessageType.Error));
            }
        }

        public static void ValidateReleaseDate(Response<Allocation> response, AllocationDto allocation)
        {
            if (!allocation.ReleaseDate.HasValue || allocation.ReleaseDate == DateTime.MinValue)
            {
                response.Messages.Add(new Message(Resources.es.AllocationManagement.Allocation.ReleaseDateIsRequired, MessageType.Error));
            }
        }
    }
}
