using System;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class CostCenterService : ICostCenterService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CostCenterService> logger;

        public CostCenterService(IUnitOfWork unitOfWork, ILogMailer<CostCenterService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
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

                response.AddSuccess(Resources.AllocationManagement.CostCenter.Save);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public Response Edit(int id, string description)
        {
            var response = new Response();

            var costCenter = CostCenterValidationHelper.Exist(response, id, unitOfWork);
            if (response.HasErrors()) return response;

            costCenter.Description = description;

            CostCenterValidationHelper.ValidateDescription(response, costCenter);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.CostCenterRepository.Update(costCenter);
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.CostCenter.Save);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public ICollection<CostCenter> GetAll()
        {
            return unitOfWork.CostCenterRepository.GetAll();
        }

        public Response Active(int id, bool active)
        {
            var response = new Response();

            var costCenter = CostCenterValidationHelper.Exist(response, id, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                costCenter.Active = active;

                unitOfWork.CostCenterRepository.Update(costCenter);
                unitOfWork.Save();

                response.AddSuccess(active ? Resources.AllocationManagement.CostCenter.Enabled : Resources.AllocationManagement.CostCenter.Disabled);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public Response<CostCenter> GetById(int id)
        {
            var response = new Response<CostCenter>();

           response.Data = CostCenterValidationHelper.Exist(response, id, unitOfWork);

            return response;
        }
    }
}
