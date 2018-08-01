using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Rrhh;
using Sofco.Framework.ValidationHelpers.Rrhh;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.Rrhh
{
    public class LicenseTypeService : ILicenseTypeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<LicenseTypeService> logger;

        public LicenseTypeService(IUnitOfWork unitOfWork, ILogMailer<LicenseTypeService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public ICollection<LicenseType> GetOptions()
        {
            return unitOfWork.LicenseTypeRepository.GetAllReadOnly();
        }

        public Response UpdateLicenseTypeDays(int typeId, int value)
        {
            var response = new Response();

            LicenseTypeValidationHandler.ValidateValue(value, response);
            var licenseType = LicenseTypeValidationHandler.Find(response, typeId, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                licenseType.Days = value;
                unitOfWork.LicenseTypeRepository.Update(licenseType);
                unitOfWork.Save();

                response.AddSuccess(Resources.Rrhh.License.LicenseTypeUpdate);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }
    }
}
