using System;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Sofco.Core.FileManager;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class SalaryAdvancementService : ISalaryAdvancementService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<SalaryAdvancementService> logger;
        private readonly IAdvancemenValidation validation;
        private readonly AppSetting settings;
        private readonly ISalaryAdvancementFileManager salaryAdvancementFileManager;

        public SalaryAdvancementService(IUnitOfWork unitOfWork,
            ILogMailer<SalaryAdvancementService> logger,
            IAdvancemenValidation validation,
            ISalaryAdvancementFileManager salaryAdvancementFileManager,
            IOptions<AppSetting> settingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.salaryAdvancementFileManager = salaryAdvancementFileManager;
            this.validation = validation;
            this.settings = settingOptions.Value;
        }

        public Response<IList<SalaryAdvancementModel>> Get()
        {
            var response = new Response<IList<SalaryAdvancementModel>> { Data = new List<SalaryAdvancementModel>() };

            var advancements = unitOfWork.AdvancementRepository.GetSalaryResume(settings.SalaryWorkflowId, settings.WorkflowStatusApproveId);

            foreach (var advancement in advancements)
            {
                var modelExist = response.Data.SingleOrDefault(x => x.UserId == advancement.UserApplicantId);

                if (modelExist == null)
                {
                    var model = new SalaryAdvancementModel();

                    model.UserId = advancement.UserApplicantId;
                    model.UserName = advancement.UserApplicant.Name;
                    model.Email = advancement.UserApplicant.Email;
                    model.TotalAmount = advancement.Ammount;

                    response.Data.Add(model);
                }
                else
                {
                    modelExist.TotalAmount += advancement.Ammount;
                }
            }

            foreach (var model in response.Data)
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmailWithDiscounts(model.Email);

                model.DiscountedAmount = employee.SalaryDiscounts.Sum(x => x.Amount);
            }

            return response;
        }

        public void Import(IFormFile file, Response response)
        {
            try
            {
                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);

                salaryAdvancementFileManager.Import(memoryStream);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.SaveFileError);
            }
        }
    }
}
