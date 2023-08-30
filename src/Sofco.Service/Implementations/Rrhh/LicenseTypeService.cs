using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Rrhh;
using Sofco.Framework.ValidationHelpers.Rrhh;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Utils;
using Sofco.Common.Security;
using Sofco.Common.Security.Interfaces;
using System.Collections.ObjectModel;

namespace Sofco.Service.Implementations.Rrhh
{
    public class LicenseTypeService : ILicenseTypeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<LicenseTypeService> logger;
        private readonly ISessionManager sessionManager;

        public LicenseTypeService(IUnitOfWork unitOfWork, 
                                  ILogMailer<LicenseTypeService> logger,
                                  ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.sessionManager = sessionManager;
        }

        public ICollection<LicenseType> GetOptions()
        {            
            try
            {
                var types = unitOfWork.LicenseTypeRepository.GetAllActivesReadOnly();
                return types;
            }
            catch (Exception e)
            {
                logger.LogError(e);
                throw e;
            }
        }

        public ICollection<LicenseType> GetOptionsRrhh()
        {
            try
            {
                var types = new List<LicenseType>();
                var email = sessionManager.GetUserEmail();
                var isRrhh = unitOfWork.UserRepository.HasRrhhGroup(email);
                if (isRrhh)
                {
                    types = unitOfWork.LicenseTypeRepository.GetAllListRrhh();
                }
                else
                {
                    types = unitOfWork.LicenseTypeRepository.GetAllListEmp();
                }

                return types;
            }
            catch (Exception e)
            {
                logger.LogError(e);
                throw e;
            }
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
