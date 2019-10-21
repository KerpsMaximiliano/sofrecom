using Sofco.Core.DAL;
using Sofco.Domain.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public class HolidaysLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Domain.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var user = unitOfWork.EmployeeRepository.GetById(domain.EmployeeId);

            //Item 1 = Working Days
            //Item 2 = Total Days 
            var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate, unitOfWork);

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
