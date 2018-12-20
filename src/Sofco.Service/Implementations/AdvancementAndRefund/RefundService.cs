﻿using System;
using System.Collections.Generic;
using AutoMapper;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Logger;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.Billing;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.Service.Implementations.AdvancementAndRefund
{
    public class RefundService : IRefundService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<RefundService> logger;
        private readonly IRefundValidation validation;
        private readonly AppSetting settings;
        private readonly IWorkflowStateRepository workflowStateRepository;
        private readonly IMapper mapper;
        private readonly FileConfig fileConfig;
        private readonly IRefundRepository refundRepository;
        private readonly IUserData userData;

        public RefundService(IUnitOfWork unitOfWork,
            ILogMailer<RefundService> logger,
            IRefundValidation validation,
            IOptions<AppSetting> settingOptions, 
            IWorkflowStateRepository workflowStateRepository,
            IOptions<FileConfig> fileOptions,
            IMapper mapper, 
            IRefundRepository refundRepository, 
            IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.validation = validation;
            this.workflowStateRepository = workflowStateRepository;
            this.mapper = mapper;
            this.refundRepository = refundRepository;
            this.userData = userData;
            fileConfig = fileOptions.Value;
            settings = settingOptions.Value;
        }

        public Response<string> Add(RefundModel model)
        {
            var response = new Response<string>();

            validation.ValidateAdd(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain();
                domain.StatusId = settings.WorkflowStatusDraft;
                domain.AdvancementRefunds = new List<AdvancementRefund>();

                foreach (var advancement in model.Advancements)
                {
                    domain.AdvancementRefunds.Add(new AdvancementRefund { AdvancementId = advancement, Refund = domain });
                }

                unitOfWork.RefundRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Refund.AddSuccess);

                response.Data = domain.Id.ToString();
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public async Task<Response<File>> AttachFile(int refundId, Response<File> response, IFormFile file)
        {
            var refund = unitOfWork.RefundRepository.Get(refundId);

            if (refund == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.NotFound);
                return response;
            }

            var user = userData.GetCurrentUser();

            var fileToAdd = new File();
            var lastDotIndex = file.FileName.LastIndexOf('.');

            fileToAdd.FileName = file.FileName;
            fileToAdd.FileType = file.FileName.Substring(lastDotIndex);
            fileToAdd.InternalFileName = Guid.NewGuid();
            fileToAdd.CreationDate = DateTime.UtcNow;
            fileToAdd.CreatedUser = user.UserName;

            var refundFile = new RefundFile
            {
                File = fileToAdd,
                RefundId = refundId
            };

            try
            {
                var fileName = $"{fileToAdd.InternalFileName.ToString()}{fileToAdd.FileType}";

                using (var fileStream = new FileStream(Path.Combine(fileConfig.RefundPath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                unitOfWork.RefundRepository.InsertFile(refundFile);
                unitOfWork.Save();

                response.Data = refundFile.File;
                response.AddSuccess(Resources.Billing.PurchaseOrder.FileAdded);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.SaveFileError);
                logger.LogError(e);
            }

            return response;
        }

        public Response<RefundEditModel> Get(int id)
        {
            var response = new Response<RefundEditModel>();

            var refund = unitOfWork.RefundRepository.GetFullById(id);

            if (refund == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.NotFound);
                return response;
            }

            response.Data = new RefundEditModel(refund);

            return response;
        }

        public Response DeleteFile(int id, int fileId)
        {
            var response = new Response();

            var refundFile = unitOfWork.RefundRepository.GetFile(id, fileId);

            if (refundFile == null)
            {
                response.AddError(Resources.Common.FileNotFound);
                return response;
            }

            try
            {
                var file = refundFile.File;

                unitOfWork.RefundRepository.DeleteFile(refundFile);

                var fileName = $"{file.InternalFileName.ToString()}{file.FileType}";
                var path = Path.Combine(fileConfig.RefundPath, fileName);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Common.FileDeleted);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.GeneralError);
                logger.LogError(e);
            }

            return response;
        }

        public Response<IList<WorkflowHistoryModel>> GetHistories(int id)
        {
            var histories = unitOfWork.RefundRepository.GetHistories(id);

            var response = new Response<IList<WorkflowHistoryModel>>();

            response.Data = histories.Select(x => new WorkflowHistoryModel(x)).ToList();

            return response;
        }

        public Response<List<WorkflowStateOptionModel>> GetStates()
        {
            var result = workflowStateRepository.GetStateByWorkflowTypeCode(settings.RefundWorkflowTypeCode);

            return new Response<List<WorkflowStateOptionModel>>
            {
                Data = Translate(result)
            };
        }

        public Response<List<RefundListResultModel>> GetByParameters(RefundListParameterModel model)
        {
            var result = refundRepository.GetByParameters(model);

            return new Response<List<RefundListResultModel>>();
        }

        private List<WorkflowStateOptionModel> Translate(List<WorkflowState> data)
        {
            return mapper.Map<List<WorkflowState>, List<WorkflowStateOptionModel>>(data);
        }
    }
}
