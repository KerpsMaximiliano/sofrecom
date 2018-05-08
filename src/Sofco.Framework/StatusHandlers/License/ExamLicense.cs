using System;
using Sofco.Core.DAL;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public class ExamLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Model.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == domain.TypeId);
            var user = unitOfWork.EmployeeRepository.GetById(domain.EmployeeId);
            var examDaysAllowTogether = unitOfWork.SettingRepository.GetByKey("ExamDaysAllowTogether");

            //Item 1 = Working Days
            //Item 2 = Total Days 
            var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate);

            if (string.IsNullOrWhiteSpace(domain.ExamDescription))
            {
                response.AddError(Resources.Rrhh.License.ExamDescriptionRequired);
            }

            if ((!domain.Final && !domain.Parcial) || (domain.Final && domain.Parcial))
            {
                response.AddError(Resources.Rrhh.License.ExamTypeRequired);
            }

            if (tupla.Item1 > Convert.ToInt32(examDaysAllowTogether.Value))
            {
                response.AddError(Resources.Rrhh.License.DaysWrong);
            }
            else
            {
                if (response.HasErrors()) return;

                domain.DaysQuantity = tupla.Item1;
                domain.DaysQuantityByLaw = tupla.Item2;

                if (user.ExamDaysTaken + tupla.Item1 > licenseType.Days)
                {
                    response.AddWarning(Resources.Rrhh.License.ExamDaysTakenExceeded);
                }
            }
        }
    }
}
