using Sofco.Core.DAL;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sofco.Framework.StatusHandlers.License
{
    public class PaternityDaysLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Domain.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == domain.TypeId);
            var employee = unitOfWork.EmployeeRepository.GetById(domain.EmployeeId);
            var paternityDaysAllowTogether = unitOfWork.SettingRepository.GetByKey("PaternityDaysAllowTogether");

            var holidays = unitOfWork.HolidayRepository.Get(domain.StartDate.Year, domain.StartDate.Month);

            if (holidays.Any(x => x.Date.Date == domain.StartDate.Date))
            {
                response.AddError(Resources.Rrhh.License.CannotStartAHoliday);
            }

            if (response.HasErrors()) return;

            //Item 1 = Working Days
            //Item 2 = Total Days 
            var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate, unitOfWork, holidays);
            
            if (tupla.Item1 > Convert.ToInt32(paternityDaysAllowTogether.Value))
            {
                response.AddError(Resources.Rrhh.License.DaysWrong);
            }
            else
            {
                if (response.HasErrors()) return;

                domain.DaysQuantity = tupla.Item1;
                domain.DaysQuantityByLaw = tupla.Item2;

                if (employee.PaternityDaysTaken + tupla.Item1 > licenseType.Days)
                {
                    response.AddWarning(Resources.Rrhh.License.PaternityDaysTakenExceeded);
                }
            }
        }
    }
}
