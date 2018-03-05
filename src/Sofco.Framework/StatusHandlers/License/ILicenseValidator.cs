using Sofco.Core.DAL;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public interface ILicenseValidator
    {
        void Validate(Response response, Model.Models.Rrhh.License domain, IUnitOfWork unitOfWork);
    }
}
