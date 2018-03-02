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

            var days = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate);

            if (days > user.HolidaysPending)
            {
                response.AddError(Resources.Rrhh.License.DaysWrong);
            }
            else
            {
                user.HolidaysPending -= days;
                unitOfWork.EmployeeRepository.Update(user);
                domain.DaysQuantity = days;
            }
        }

        private int GetNumberOfWorkingDays(DateTime start, DateTime stop)
        {
            int days = 0;
            while (start.Date <= stop.Date)
            {
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                {
                    ++days;
                }
                start = start.AddDays(1);
            }
            return days;
        }
    }
}
