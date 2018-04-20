using System;
using Sofco.Core.DAL;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public class HolidaysLicense : ILicenseValidator
    {
        public void Validate(Response response, Model.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var user = unitOfWork.EmployeeRepository.GetById(domain.EmployeeId);

            //Item 1 = Working Days
            //Item 2 = Total Days
            var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate);

            if (tupla.Item1 > user.HolidaysPending)
            {
                response.AddError(Resources.Rrhh.License.DaysWrong);
            }
            else
            {
                domain.DaysQuantity = tupla.Item1;
                domain.DaysQuantityByLaw = tupla.Item2;
            }
        }

        private Tuple<int, int> GetNumberOfWorkingDays(DateTime start, DateTime stop)
        {
            int workingDays = 0;
            int totalDays = 0;

            while (start.Date <= stop.Date)
            {
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }

                totalDays++;

                start = start.AddDays(1);
            }

            return new Tuple<int, int>(workingDays, totalDays);
        }
    }
}
