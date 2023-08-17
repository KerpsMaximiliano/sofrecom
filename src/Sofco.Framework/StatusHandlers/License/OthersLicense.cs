using Sofco.Core.DAL;
using Sofco.Domain.Utils;
using System.Linq;

namespace Sofco.Framework.StatusHandlers.License
{ 
    public class OthersLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Domain.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(domain.Comments))
            {
                response.AddError(Resources.Rrhh.License.CommentsRequired);
            }
            else
            {
                var holidays = unitOfWork.HolidayRepository.Get(domain.StartDate.Year, domain.StartDate.Month);

                if (holidays.Any(x => x.Date.Date == domain.StartDate.Date))
                {
                    response.AddError(Resources.Rrhh.License.CannotStartAHoliday);
                }

                if (response.HasErrors()) return;

                //Item 1 = Working Days
                //Item 2 = Total Days 
                var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate, unitOfWork, holidays);

                domain.DaysQuantity = tupla.Item1;
                domain.DaysQuantityByLaw = tupla.Item2;
            }
        }
    }
}
