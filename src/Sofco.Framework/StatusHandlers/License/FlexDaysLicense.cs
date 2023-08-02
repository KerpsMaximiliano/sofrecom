﻿using Sofco.Core.DAL;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Framework.StatusHandlers.License
{
    public class FlexDaysLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Domain.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == domain.TypeId);
            var employee = unitOfWork.EmployeeRepository.GetById(domain.EmployeeId);
            var flexDaysAllowTogether = unitOfWork.SettingRepository.GetByKey("FlexDaysAllowTogether");

            //Item 1 = Working Days
            //Item 2 = Total Days 
            var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate, unitOfWork);            

            if (tupla.Item1 > Convert.ToInt32(flexDaysAllowTogether.Value))
            {
                response.AddError(Resources.Rrhh.License.DaysWrong);
            }
            else
            {
                if (response.HasErrors()) return;

                domain.DaysQuantity = tupla.Item1;
                domain.DaysQuantityByLaw = tupla.Item2;

                if (employee.FlexDaysTaken + tupla.Item1 > licenseType.Days)
                {
                    response.AddWarning(Resources.Rrhh.License.FlexDaysTakenExceeded);
                }
            }
        }
    }
}
