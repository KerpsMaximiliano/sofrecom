using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.DAL.AdvancementAndRefund;
using Sofco.Core.DAL.Workflow;
using Sofco.Core.Data.Admin;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Validations.AdvancementAndRefund;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IRoleManager roleManager;
        private readonly IRefundRepository refundRepository;
        private readonly IUserData userData;

        public RefundService(IUnitOfWork unitOfWork,
            ILogMailer<RefundService> logger,
            IRefundValidation validation,
            IOptions<AppSetting> settingOptions,
            IWorkflowStateRepository workflowStateRepository,
            IOptions<FileConfig> fileOptions,
            IMapper mapper,
            IRoleManager roleManager,
            IRefundRepository refundRepository,
            IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.validation = validation;
            this.workflowStateRepository = workflowStateRepository;
            this.mapper = mapper;
            this.roleManager = roleManager;
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
                domain.InWorkflowProcess = true;

                var workflow = unitOfWork.WorkflowRepository.GetLastByType(settings.RefundWorkflowId);

                domain.WorkflowId = workflow.Id;

                domain.AdvancementRefunds = new List<AdvancementRefund>();

                if (model.Advancements != null)
                {
                    foreach (var advancementId in model.Advancements)
                    {
                        domain.AdvancementRefunds.Add(new AdvancementRefund { Advancement = unitOfWork.AdvancementRepository.Get(advancementId), Refund = domain });
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

                if (model.Advancements != null)
                {
                    foreach (var advancementId in model.Advancements)
                    {
                        domain.AdvancementRefunds.Add(new AdvancementRefund { Advancement = unitOfWork.AdvancementRepository.Get(advancementId), Refund = domain });
                    }
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

        public Response<List<Option>> GetStates()
        {
            var result = workflowStateRepository.GetStateByWorkflowTypeCode(settings.RefundWorkflowTypeCode)
                .Select(x => new Option { Id = x.Id, Text = x.Name })
                .ToList();

            if (result.All(x => x.Id != settings.WorkflowStatusFinalizedId))
            {
                var finalizeState = workflowStateRepository.Get(settings.WorkflowStatusFinalizedId);

                result.Add(new Option { Id = finalizeState.Id, Text = finalizeState.Name });
            }

            var draft = result.SingleOrDefault(x => x.Id == settings.WorkflowStatusDraft);

            if (draft != null) result.Remove(draft);

            return new Response<List<Option>>
            {
                Data = result
            };
        }

        public Response<List<RefundListResultModel>> GetByParameters(RefundListParameterModel model)
        {
            ValidParameter(model);

            var currentUser = userData.GetCurrentUser();
            var hasAllAccess = roleManager.HasAccessForRefund();

            if (!hasAllAccess)
            {
                model.UserApplicantIds = GetUserApplicantIdsByCurrentManager();
            }

            var result = refundRepository.GetByParameters(model, settings.WorkflowStatusDraft);

            var response = new Response<List<RefundListResultModel>> { Data = new List<RefundListResultModel>() };

            if (hasAllAccess)
            {
                response.Data = Translate(result);
            }
            else
            {
                var employeeDicc = new Dictionary<string, string>();
                var employeeNameDicc = new Dictionary<string, string>();

                foreach (var refund in result)
                {
                    if (ValidateAnalyticManagerAccess(refund, currentUser) || ValidateSectorAccess(refund, currentUser) || ValidateUserAccess(refund, currentUser))
                    {
                        if (response.Data.All(x => x.Id != refund.Id))
                        {
                            var mapped = mapper.Map<Refund, RefundListResultModel>(refund);

                            var advancementsAndRefunds = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(refund.Id);

                            var diff = advancementsAndRefunds.Item2.Sum(x => x.Ammount) - advancementsAndRefunds.Item1.Sum(x => x.TotalAmmount);

                            if (diff > 0)
                            {
                                mapped.CompanyRefund = diff;
                            }

                            mapped.IsCreditCard = refund.CreditCardId > 0;

                            if (refund.Histories.Any())
                            {
                                var lastHistory = refund.Histories.OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                                if (lastHistory != null) mapped.LastUpdate = lastHistory.CreatedDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
                            }
                            else
                            {
                                mapped.LastUpdate = refund.CreationDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
                            }

                            if (!employeeDicc.ContainsKey(refund.UserApplicant.Email))
                            {
                                var employee = unitOfWork.EmployeeRepository.GetByEmail(refund.UserApplicant.Email);

                                employeeDicc.Add(refund.UserApplicant.Email, employee?.Bank);
                                employeeNameDicc.Add(refund.UserApplicant.Email, employee?.Name);
                            }

                            mapped.Bank = employeeDicc[refund.UserApplicant.Email];
                            mapped.Bank = employeeNameDicc[refund.UserApplicant.Email];

                            response.Data.Add(mapped);
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.Bank))
            {
                response.Data = response.Data.Where(d => d.Bank == model.Bank).ToList();
            }

            return response;
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

        private List<RefundListResultModel> Translate(List<Refund> data)
        {
            var employeeDicc = new Dictionary<string, string>();
            var employeeNameDicc = new Dictionary<string, string>();
            var result = new List<RefundListResultModel>();

            foreach (var refund in data)
            {
                var item = mapper.Map<Refund, RefundListResultModel>(refund);

                if (refund.AdvancementRefunds.Any())
                {
                    var advancementsAndRefunds = unitOfWork.RefundRepository.GetAdvancementsAndRefundsByRefundId(refund.Id);

                    var diff = advancementsAndRefunds.Item2.Sum(x => x.Ammount) - advancementsAndRefunds.Item1.Sum(x => x.TotalAmmount);

                    if (diff > 0)
                        item.CompanyRefund = diff;
                    else
                        item.UserRefund = diff * -1;
                }
                else
                {
                    item.CompanyRefund = refund.Details.Sum(x => x.Ammount);
                }

                item.IsCreditCard = refund.CreditCardId > 0;

                if (refund.Histories.Any())
                {
                    var lastHistory = refund.Histories.OrderByDescending(x => x.CreatedDate).FirstOrDefault();

                    if (lastHistory != null) item.LastUpdate = lastHistory.CreatedDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
                }
                else
                {
                    item.LastUpdate = refund.CreationDate.AddHours(-3).ToString("dd/MM/yyyy HH:mm");
                }

                if (!employeeDicc.ContainsKey(refund.UserApplicant.Email))
                {
                    var employee = unitOfWork.EmployeeRepository.GetByEmail(refund.UserApplicant.Email);

                    employeeDicc.Add(refund.UserApplicant.Email, employee?.Bank);
                    employeeNameDicc.Add(refund.UserApplicant.Email, employee?.Name);
                }

                item.UserApplicantName = employeeNameDicc[refund.UserApplicant.Email];
                item.Bank = employeeDicc[refund.UserApplicant.Email];

                result.Add(item);
            }

            return result;
        }

        private bool ValidateSectorAccess(Refund entity, UserLiteModel currentUser)
        {
            var hasAccess = false;

            var director = unitOfWork.AnalyticRepository.GetDirector(entity.AnalyticId);

            if (director != null)
            {
                if (director.Id == currentUser.Id) hasAccess = true;

                var userApprovers = unitOfWork.UserApproverRepository.GetByAnalyticAndUserId(director.Id, entity.AnalyticId, UserApproverType.Refund);

                if (userApprovers.Select(x => x.ApproverUserId).Contains(currentUser.Id))
                {
                    hasAccess = true;
                }
            }

            return hasAccess;
        }

        private bool ValidateUserAccess(Refund entity, UserLiteModel currentUser)
        {
            bool hasAccess = entity.Status.ActualTransitions.Any(x => x.WorkflowStateAccesses.Any(s => s.UserSource.SourceId == currentUser.Id && s.UserSource.Code == settings.UserUserSource && s.AccessDenied == false));

            return hasAccess;
        }

        private bool ValidateAnalyticManagerAccess(Refund entity, UserLiteModel currentUser)
        {
            var hasAccess = false;

            if (entity.Analytic != null && entity.Analytic.ManagerId.HasValue)
            {
                if (entity.Analytic.ManagerId.Value == currentUser.Id)
                {
                    hasAccess = true;
                }

                var userApprovers = unitOfWork.UserApproverRepository.GetByAnalyticAndUserId(entity.Analytic.ManagerId.Value, entity.AnalyticId, UserApproverType.Refund);

                if (userApprovers.Select(x => x.ApproverUserId).Contains(currentUser.Id))
                {
                    hasAccess = true;
                }
            }

            return hasAccess;
        }

        public Response<IList<Option>> GetAnalitycs()
        {
            var response = new Response<IList<Option>> { Data = new List<Option>() };

            var currentUser = userData.GetCurrentUser();

            if (roleManager.IsManager())
            {
                var analytics = unitOfWork.AnalyticRepository.GetAnalyticLiteByManagerId(currentUser.Id);

                foreach (var analytic in analytics)
                    response.Data.Add(new Option { Id = analytic.Id, Text = $"{analytic.Title} - {analytic.Name}" });
            }

            if (roleManager.IsDirector())
            {
                var analytics = unitOfWork.AnalyticRepository.GetByDirectorId(currentUser.Id);

                foreach (var analytic in analytics)
                    response.Data.Add(new Option { Id = analytic.Id, Text = $"{analytic.Title} - {analytic.Name}" });
            }

            return response;
        }

        public Response<int> Delegate(int userId)
        {
            var response = new Response<int>();

            if (!unitOfWork.UserRepository.ExistById(userId))
            {
                response.AddError(Resources.Admin.User.NotFound);
                return response;
            }

            var currentUser = userData.GetCurrentUser();

            if (currentUser.Id == userId)
            {
                response.AddError(Resources.AdvancementAndRefund.Refund.CurrentUserEqualsDelegate);
                return response;
            }

            try
            {
                var userDelegate = new UserDelegate
                {
                    Created = DateTime.UtcNow,
                    CreatedUser = currentUser.UserName,
                    Modified = DateTime.UtcNow,
                    SourceId = userId,
                    Type = UserDelegateType.RefundAdd,
                    UserId = currentUser.Id
                };

                unitOfWork.UserDelegateRepository.Insert(userDelegate);
                unitOfWork.Save();

                response.AddSuccess(Resources.AdvancementAndRefund.Refund.DelegateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response DeleteDelegate(List<int> ids)
        {
            var response = new Response();

            try
            {
                foreach (var id in ids)
                {
                    var userDelegate = unitOfWork.UserDelegateRepository.Get(id);

                    if (userDelegate != null)
                    {
                        unitOfWork.UserDelegateRepository.Delete(userDelegate);
                    }
                }

                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<Option>> GetDelegates()
        {
            var response = new Response<IList<Option>> { Data = new List<Option>() };

            var delegates = unitOfWork.UserDelegateRepository.GetByUserId(userData.GetCurrentUser().Id, UserDelegateType.RefundAdd);

            foreach (var userDelegate in delegates)
            {
                var user = userData.GetUserLiteById(userDelegate.SourceId.GetValueOrDefault());

                response.Data.Add(new Option
                {
                    Id = userDelegate.Id,
                    Text = user.Name
                });
            }

            return response;
        }
    }
}
