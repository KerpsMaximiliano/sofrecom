using Sofco.Core.DAL;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public class SpecialLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Model.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(domain.Comments))
            {
                response.AddError(Resources.Rrhh.License.CommentsRequired);
            }
            else
            {
                var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == domain.TypeId);

                //Item 1 = Working Days
                //Item 2 = Total Days 
                var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate);

                if (licenseType.Days > 0 && tupla.Item2 > licenseType.Days)
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
}
