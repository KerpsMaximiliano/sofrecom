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
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Models.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;
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

                if (model.Advancements != null)
                {
                    foreach (var advancement in model.Advancements)
                    {
                        domain.AdvancementRefunds.Add(new AdvancementRefund { Advancement = unitOfWork.AdvancementRepository.GetById(advancement), Refund = domain });
                    }
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

        public Response Update(RefundModel model)
        {
            var response = validation.ValidateUpdate(model);

            if (response.HasErrors()) return response;

            try
            {
                var domain = unitOfWork.RefundRepository.GetById(model.Id);
                model.UpdateDomain(domain);

                domain.AdvancementRefunds = new List<AdvancementRefund>();

                unitOfWork.RefundRepository.Update(domain);
                unitOfWork.Save();

                foreach (var advancement in model.Advancements)
                {
                    domain.AdvancementRefunds.Add(new AdvancementRefund { Advancement = unitOfWork.AdvancementRepository.GetById(advancement), Refund = domain });
                }

                unitOfWork.RefundRepository.Update(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Refund.UpdateSuccess);
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

        public Response Delete(int id)
        {
            var response = new Response();

            var domain = unitOfWork.RefundRepository.Get(id);

            if (domain == null)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.NotFound);
                return response;
            }

            if (domain.StatusId != settings.WorkflowStatusDraft)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.CannotDelete);
                return response;
            }

            try
            {
                unitOfWork.RefundRepository.Delete(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Refund.DeleteSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

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
            ValidParameter(model);

            var currentUser = userData.GetCurrentUser();

            if (!HasAllAccess())
            {
                model.UserApplicantIds = GetUserApplicantIdsByCurrentManager();
            }

            var result = refundRepository.GetByParameters(model);

            if (HasAllAccess())
            {
                return new Response<List<RefundListResultModel>>
                {
                    Data = Translate(result)
                };
            }
            else
            {
                var response = new Response<List<RefundListResultModel>> { Data = new List<RefundListResultModel>() };

                foreach (var refund in result)
                {
                    if (ValidateManagerAccess(refund, currentUser))
                    {
                        if (response.Data.All(x => x.Id != refund.Id))
                        {
                            var mapped = mapper.Map<Refund, RefundListResultModel>(refund);

                            response.Data.Add(mapped);
                        }
                    }
                }

                response.Data = ResolveManager(response.Data);

                return response;
            }
        }

        private List<int> GetUserApplicantIdsByCurrentManager()
        {
            var currentUser = userData.GetCurrentUser();

            var employees = unitOfWork.EmployeeRepository.GetByManagerId(currentUser.Id);

            var emails = employees
                .Where(s => s.Email != string.Empty)
                .Select(s => s.Email)
                .Distinct()
                .ToList();

            return unitOfWork.UserRepository
                .GetUserLiteByEmails(emails)
                .Select(s => s.Id)
                .ToList();
        }

        private void ValidParameter(RefundListParameterModel model)
        {
            if (!model.DateSince.HasValue)
            {
                model.DateSince = DateTime.UtcNow.AddYears(-1);
            }
        }

        private List<WorkflowStateOptionModel> Translate(List<WorkflowState> data)
        {
            return mapper.Map<List<WorkflowState>, List<WorkflowStateOptionModel>>(data);
        }

        private List<RefundListResultModel> Translate(List<Refund> data)
        {
            var result = mapper.Map<List<Refund>, List<RefundListResultModel>>(data);

            return ResolveManager(result);
        }

        private List<RefundListResultModel> ResolveManager(List<RefundListResultModel> models)
        {
            var userIds = models.Select(s => s.UserApplicantId).Distinct();

            foreach (var userId in userIds)
            {
                var user = userData.GetUserLiteById(userId);

                var employee = unitOfWork.EmployeeRepository.GetByEmail(user.Email);

                foreach (var refundListResultModel in models.Where(s => s.UserApplicantId == userId))
                {
                    refundListResultModel.ManagerName = employee.Manager?.Name;
                }
            }

            return models;
        }

        private bool HasAllAccess()
        {
            var currentUser = userData.GetCurrentUser();

            var hasDirectorGroup = unitOfWork.UserRepository.HasDirectorGroup(currentUser.Email);
            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUser.Email);
            var hasRrhhGroup = unitOfWork.UserRepository.HasRrhhGroup(currentUser.Email);
            var hasGafGroup = unitOfWork.UserRepository.HasGafGroup(currentUser.Email);
            var hasComplianceGroup = unitOfWork.UserRepository.HasComplianceGroup(currentUser.Email);

            var hasAllAccess = hasDirectorGroup || hasRrhhGroup || hasGafGroup || hasDafGroup || hasComplianceGroup;

            return hasAllAccess;
        }

        private bool ValidateManagerAccess(Refund entity, UserLiteModel currentUser)
        {
            if (entity.AuthorizerId.HasValue && entity.AuthorizerId.Value == currentUser.Id)
            {
                return true;
            }
            else
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmail(entity.UserApplicant.Email);

                if (employee.ManagerId.HasValue && employee.Manager != null && employee.ManagerId.Value == currentUser.Id)
                {
                    return true;
                }
            }

            return false;
        }

        public Response<IList<PaymentPendingModel>> GetAllPaymentPending()
        {
            var response = new Response<IList<PaymentPendingModel>>();
            response.Data = new List<PaymentPendingModel>();

            var employeeDicc = new Dictionary<string, string>();

            var currentUser = userData.GetCurrentUser();

            var hasDafGroup = unitOfWork.UserRepository.HasDafGroup(currentUser.Email);

            if (!hasDafGroup) return response;

            var refunds = unitOfWork.RefundRepository.GetAllPaymentPending(settings.WorkFlowStatePaymentPending);

            response.Data = refunds.Select(x =>
            {
                var item = new PaymentPendingModel
                {
                    Id = x.Id,
                    UserApplicantId = x.UserApplicantId,
                    UserApplicantDesc = x.UserApplicant?.Name,
                    CurrencyId = x.CurrencyId,
                    CurrencyDesc = x.Currency?.Text,
                    Ammount = x.TotalAmmount,
                    Type = "Reintegro"
                };

                item.Bank = GetBank(x.UserApplicant?.Email, employeeDicc);

                var transition = x.Status.ActualTransitions.FirstOrDefault();

                if (transition != null)
                {
                    item.WorkflowId = transition.WorkflowId;
                    item.NextWorkflowStateId = transition.NextWorkflowStateId;
                }

                return item;
            }).ToList();

            return response;
        }

        private string GetBank(string email, Dictionary<string, string> employeeDictionary)
        {
            if (!employeeDictionary.ContainsKey(email))
            {
                var employee = unitOfWork.EmployeeRepository.GetByEmail(email);

                employeeDictionary.Add(email, employee?.Bank);
            }

            return employeeDictionary[email];
        }
    }
}
