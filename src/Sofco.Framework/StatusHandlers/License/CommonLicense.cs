using Sofco.Core.DAL;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public class CommonLicense : ILicenseValidator
    {
        public void Validate(Response response, Model.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == domain.TypeId);

            var days = domain.EndDate.Date.Subtract(domain.StartDate.Date).Days + 1;

            if (licenseType.Days > 0 && days > licenseType.Days)
            {
                response.AddError(Resources.Rrhh.License.DaysWrong);
            }
            else
            {
                domain.DaysQuantity = days;
            }
        }
    }
}
