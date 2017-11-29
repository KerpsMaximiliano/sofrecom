using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class CostCenterService : ICostCenterService
    {
        private readonly IUnitOfWork unitOfWork;

        public CostCenterService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Response Add(CostCenter domain)
        {
            var response = new Response();

            CostCenterValidationHelper.ValidateCode(response, domain, unitOfWork.CostCenterRepository);
            CostCenterValidationHelper.ValidateLetter(response, domain);
            CostCenterValidationHelper.ValidateDescription(response, domain);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.CostCenterRepository.Insert(domain);
                unitOfWork.Save();

                response.Messages.Add(new Message(Resources.AllocationManagement.CostCenter.Save, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<CostCenter> GetAll()
        {
            return unitOfWork.CostCenterRepository.GetAll();
        }
    }
}
