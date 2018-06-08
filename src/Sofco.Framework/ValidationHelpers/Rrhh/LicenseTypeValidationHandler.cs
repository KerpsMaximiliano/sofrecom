using Sofco.Core.DAL;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.Rrhh
{
    public static class LicenseTypeValidationHandler
    {
        public static LicenseType Find(Response response, int typeId, IUnitOfWork unitOfWork)
        {
            var licenseType = unitOfWork.LicenseTypeRepository.GetSingle(x => x.Id == typeId);

            if (licenseType == null)
            {
                response.AddError(Resources.Rrhh.License.LicenseTypeNotFound);
            }

            return licenseType;
        }

        public static void ValidateValue(int value, Response response)
        {
            if (value <= 0)
            {
                response.AddError(Resources.Rrhh.License.LicenseTypeValueRequired);
            }
        }
    }
}
