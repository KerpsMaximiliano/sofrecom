using Sofco.Core.DAL;
using Sofco.Domain.Utils;

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
                //Item 1 = Working Days
                //Item 2 = Total Days 
                var tupla = GetNumberOfWorkingDays(domain.StartDate, domain.EndDate, unitOfWork);

                domain.DaysQuantity = tupla.Item1;
                domain.DaysQuantityByLaw = tupla.Item2;
            }
        }
    }
}
