using Sofco.Core.DAL;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseStatusPendingHandler : ILicenseStatusHandler
    {
        public void Validate(Response response, IUnitOfWork unitOfWork, LicenseStatusChangeModel parameters, Model.Models.Rrhh.License license)
        {
            if (!parameters.IsRrhh && license.Status != LicenseStatus.AuthPending)
            {
                response.AddError(Resources.Rrhh.License.CannotChangeStatus);
            }
        }

        public void SaveStatus(Model.Models.Rrhh.License license, LicenseStatusChangeModel model, IUnitOfWork unitOfWork)
        {
            var licenseToModif = new Model.Models.Rrhh.License { Id = license.Id, Status = model.Status };
            unitOfWork.LicenseRepository.UpdateStatus(licenseToModif);
        }

        public string GetSuccessMessage()
        {
            return Resources.Rrhh.License.PendingSuccess;
        }
    }
}
