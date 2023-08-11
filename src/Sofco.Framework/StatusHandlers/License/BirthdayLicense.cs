using Sofco.Core.DAL;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Framework.StatusHandlers.License
{
    public class BirthdayLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Domain.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == domain.TypeId);
            var employee = unitOfWork.EmployeeRepository.GetById(domain.EmployeeId);
            var birthdayDaysAllowTogether = unitOfWork.SettingRepository.GetByKey("BirthdayDaysAllowTogether");

            //Item 1 = Working Days
            //Item 2 = Total Days 
            var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate, unitOfWork);
            
            if (tupla.Item1 > Convert.ToInt32(birthdayDaysAllowTogether.Value))
            {
                response.AddError(Resources.Rrhh.License.DaysWrong);
            }
            else
            {
                if (domain.StartDate.Year >= 2024)
                {
                    if (employee.Birthday.Value.Month != domain.StartDate.Month)
                        response.AddError(Resources.Rrhh.License.InvalidBirthdayDaysMonth);
                }

                if (domain.StartDate.Year == 2023)
                {
                    if (employee.Birthday.Value.Month > 8)
                    {
                        if (employee.Birthday.Value.Month != domain.StartDate.Month)
                            response.AddError(Resources.Rrhh.License.InvalidBirthdayDaysMonth);
                    }
                }

                if (employee.BirthdayDaysTaken + tupla.Item1 > licenseType.Days)
                {
                    response.AddError(Resources.Rrhh.License.BirthdayDaysTakenExceeded);
                }

                if (response.HasErrors()) return;

                domain.DaysQuantity = tupla.Item1;
                domain.DaysQuantityByLaw = tupla.Item2;                
            }
        }
    }


}
