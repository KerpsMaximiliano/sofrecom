using Sofco.Core.DAL;
using Sofco.Domain.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public interface ILicenseValidator
    {
        void Validate(Response response, Domain.Models.Rrhh.License domain, IUnitOfWork unitOfWork);
    }
}
