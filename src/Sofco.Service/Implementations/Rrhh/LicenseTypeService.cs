using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Rrhh;
using Sofco.Model.Models.AllocationManagement;

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
    }
}
