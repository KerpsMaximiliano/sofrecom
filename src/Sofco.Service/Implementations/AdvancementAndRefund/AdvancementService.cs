using System;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class AdvancementService: IAdvancementService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<AdvancementService> logger;
        private readonly IAdvancemenValidation validation;

        public AdvancementService(IUnitOfWork unitOfWork, ILogMailer<AdvancementService> logger, IAdvancemenValidation validation)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.validation = validation;
        }

        public Response Add(AdvancementModel model)
        {
            var response = validation.ValidateAdd(model);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain();
                unitOfWork.AdvancementRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Advancement.AddSuccess);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }
    }
}
