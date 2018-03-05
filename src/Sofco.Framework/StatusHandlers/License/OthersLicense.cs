using Sofco.Core.DAL;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public class OthersLicense : ILicenseValidator
    {
        public void Validate(Response response, Model.Models.Rrhh.License domain, IUnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(domain.Comments))
            {
                response.AddError(Resources.Rrhh.License.CommentsRequired);
            }
            else
            {
                var days = domain.EndDate.Date.Subtract(domain.StartDate.Date).Days + 1;
                domain.DaysQuantity = days;
            }
        }
    }
}
