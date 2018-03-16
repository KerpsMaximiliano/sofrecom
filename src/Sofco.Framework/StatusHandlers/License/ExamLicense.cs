using System;
using Sofco.Core.DAL;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public class ExamLicense : ILicenseValidator
    {
        public void Validate(Response response, Model.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == domain.TypeId);
            var user = unitOfWork.EmployeeRepository.GetById(domain.EmployeeId);
            var examDaysAllowTogether = unitOfWork.GlobalSettingRepository.GetByKey("ExamDaysAllowTogether");

            var days = domain.EndDate.Date.Subtract(domain.StartDate.Date).Days + 1;

            if (string.IsNullOrWhiteSpace(domain.ExamDescription))
            {
                response.AddError(Resources.Rrhh.License.ExamDescriptionRequired);
            }

            if (!domain.Final && !domain.Parcial)
            {
                response.AddError(Resources.Rrhh.License.ExamTypeRequired);
            }

            if (days > Convert.ToInt32(examDaysAllowTogether.Value))
            {
                response.AddError(Resources.Rrhh.License.DaysWrong);
            }
            else
            {
                if (response.HasErrors()) return;

                //user.ExamDaysTaken += days;
                //unitOfWork.EmployeeRepository.Update(user);

                domain.DaysQuantity = days;

                if (user.ExamDaysTaken + days > licenseType.Days)
                {
                    response.AddWarning(Resources.Rrhh.License.ExamDaysTakenExceeded);
                }
            }
        }
    }
}
