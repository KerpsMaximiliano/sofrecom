using Sofco.Core.DAL;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using System.Linq;

namespace Sofco.Framework.StatusHandlers.License
{
    public class HolidaysLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Domain.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var user = unitOfWork.EmployeeRepository.GetById(domain.EmployeeId);
            var holidays = unitOfWork.HolidayRepository.Get(domain.StartDate.Year, domain.StartDate.Month);

            if (holidays.Any(x => x.Date.Date == domain.StartDate.Date))
            {
                response.AddError(Resources.Rrhh.License.CannotStartAHoliday);
            }

            if (response.HasErrors()) return;

            //Item 1 = Working Days
            //Item 2 = Total Days 
            var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate, unitOfWork, holidays);

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
    }
}
