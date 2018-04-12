using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Models.Rrhh;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface ILicenseStatusHandler
    {
        void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel model, License license);
        void SaveStatus(License license, LicenseStatusChangeModel model, IUnitOfWork unitOfWork);
        string GetSuccessMessage();
        IMailData GetEmailData(License license, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters);
    }
}
