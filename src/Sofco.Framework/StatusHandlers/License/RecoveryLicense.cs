using Sofco.Core.DAL;
using Sofco.Domain.Utils;


namespace Sofco.Framework.StatusHandlers.License
{
    public class RecoveryLicense : LicenseValidator, ILicenseValidator
    {
        public void Validate(Response response, Domain.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(domain.Comments))
            {
                response.AddError(Resources.Rrhh.License.DetailPeriodRequired);
            }
            else
            {
                var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == domain.TypeId);

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
