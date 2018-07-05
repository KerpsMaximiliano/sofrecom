﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;
using File = Sofco.Model.Models.Common.File;
using PurchaseOrder = Sofco.Model.Models.Billing.PurchaseOrder;

namespace Sofco.Service.Implementations.Billing
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<PurchaseOrderService> logger;
        private readonly FileConfig fileConfig;
        private readonly IUserData userData;
        private readonly IPurchaseOrderStatusFactory purchaseOrderStatusFactory;

        public PurchaseOrderService(IUnitOfWork unitOfWork, 
            ILogMailer<PurchaseOrderService> logger, 
            IOptions<FileConfig> fileOptions,
            IPurchaseOrderStatusFactory purchaseOrderStatusFactory,
            IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            fileConfig = fileOptions.Value;
            this.userData = userData;
            this.purchaseOrderStatusFactory = purchaseOrderStatusFactory;
        }

        public Response<PurchaseOrder> Add(PurchaseOrderModel model)
        {
            var response = new Response<PurchaseOrder>();

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain(userData.GetCurrentUser().UserName);

                var history = GetHistory(domain, new PurchaseOrderStatusParams());
                history.To = PurchaseOrderStatus.Draft;

                domain.Histories.Add(history);

                unitOfWork.PurchaseOrderRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.SaveSuccess); 
                
                response.Data = domain;
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
                return response;
            }

            try
            {
                foreach (var analyticId in model.AnalyticIds)
                {
                    var purchaseOrderAnalytic = new PurchaseOrderAnalytic { AnalyticId = analyticId, PurchaseOrderId = response.Data.Id };
                    unitOfWork.PurchaseOrderRepository.AddPurchaseOrderAnalytic(purchaseOrderAnalytic);
                }

                unitOfWork.Save();
            }
            catch (Exception e)
            {
                response.AddWarning(Resources.Billing.PurchaseOrder.ErrorToSaveAnalytics);
                logger.LogError(e);
            }

            return response;
        }

        public Response Update(PurchaseOrderModel model)
        {
            var response = new Response<PurchaseOrder>();

            PurchaseOrderValidationHelper.Exist(response, model, unitOfWork);

            if (response.HasErrors()) return response;

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = PurchaseOrderValidationHelper.FindWithAnalytic(model.Id, response, unitOfWork);

                model.UpdateDomain(domain, userData.GetCurrentUser().UserName);

                var history = GetHistory(domain, new PurchaseOrderStatusParams());
                history.To = PurchaseOrderStatus.Draft;

                domain.Histories.Add(history);

                domain.Status = PurchaseOrderStatus.Draft;

                var aux = domain.PurchaseOrderAnalytics.ToList();

                foreach (var orderAnalytic in aux)
                {
                    if (model.AnalyticIds.All(x => x != orderAnalytic.AnalyticId))
                    {
                        domain.PurchaseOrderAnalytics.Remove(orderAnalytic);
                    }
                }

                foreach (var analyticId in model.AnalyticIds)
                {
                    if (domain.PurchaseOrderAnalytics.All(x => x.AnalyticId != analyticId))
                    {
                        domain.PurchaseOrderAnalytics.Add(new PurchaseOrderAnalytic { AnalyticId = analyticId, PurchaseOrderId = domain.Id });
                    }
                }

                unitOfWork.PurchaseOrderRepository.Update(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.UpdateSuccess);

                response.Data = domain;
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public Response UpdateSolfac(int id, int solfacId)
        {
            var response = new Response();

            PurchaseOrderValidationHelper.Exist(response, id, unitOfWork);
            SolfacValidationHelper.ValidateIfExist(solfacId, unitOfWork.SolfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.PurchaseOrderRepository.UpdateInSolfac(id, solfacId);
                response.AddSuccess(Resources.Billing.PurchaseOrder.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response MakeAdjustment(int id, IList<PurchaseOrderAmmountDetailModel> details)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.Find(id, response, unitOfWork);
            PurchaseOrderValidationHelper.ValidateAdjustmentAmmount(response, details);

            if (response.HasErrors()) return response;

            try
            {
                purchaseOrder.Status = PurchaseOrderStatus.Closed;
                purchaseOrder.Adjustment = true;
                unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);
                unitOfWork.PurchaseOrderRepository.UpdateAdjustment(purchaseOrder);

                foreach (var detail in purchaseOrder.AmmountDetails)
                {
                    var modelDetail = details.SingleOrDefault(x => x.CurrencyId == detail.CurrencyId);

                    if (modelDetail == null) continue;

                    detail.Adjustment = modelDetail.Adjustment;
                    unitOfWork.PurchaseOrderRepository.UpdateDetail(detail);
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Billing.PurchaseOrder.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response ChangeStatus(int id, PurchaseOrderStatusParams model)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.FindLite(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            var statusHandler = purchaseOrderStatusFactory.GetInstance(purchaseOrder.Status);

            try
            {
                // Validate Status
                statusHandler.Validate(response, model, purchaseOrder);

                if (response.HasErrors()) return response;

                var history = GetHistory(purchaseOrder, model);

                // Update Status
                statusHandler.Save(purchaseOrder, model);

                // Add History
                history.To = purchaseOrder.Status;
                unitOfWork.PurchaseOrderRepository.AddHistory(history);

                // Save
                unitOfWork.Save();
                response.AddSuccess(statusHandler.GetSuccessMessage(model));
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Billing.PurchaseOrder.CannotChangeStatus);
            }

            try
            {
                if (response.HasErrors()) return response;
                statusHandler.SendMail(purchaseOrder, model);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddWarning(Resources.Common.ErrorSendMail);
            }

            return response;
        }

        public ICollection<PurchaseOrderHistory> GetHistories(int id)
        {
            return unitOfWork.PurchaseOrderRepository.GetHistories(id);
        }

        public Response Close(int id, PurchaseOrderStatusParams model)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.FindLite(id, response, unitOfWork);

            if (response.HasErrors()) return response;

            PurchaseOrderValidationHelper.Close(response, purchaseOrder);

            if (response.HasErrors()) return response;

            try
            {
                purchaseOrder.Status = PurchaseOrderStatus.Closed;
                unitOfWork.PurchaseOrderRepository.UpdateStatus(purchaseOrder);

                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.CloseSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<PurchaseOrderPendingModel>> GetPendings()
        {
            var response = new Response<IList<PurchaseOrderPendingModel>>
            {
                Data = new List<PurchaseOrderPendingModel>()
            };

            var user = userData.GetCurrentUser();

            var purchaseOrders = unitOfWork.PurchaseOrderRepository.GetPendings();

            var isDaf = unitOfWork.UserRepository.HasDafGroup(user.Email);
            var isCdg = unitOfWork.UserRepository.HasCdgGroup(user.Email);
            var isCompliance = unitOfWork.UserRepository.HasComplianceGroup(user.Email);

            foreach (var purchaseOrder in purchaseOrders)
            {
                if (isCdg)
                {
                    response.Data.Add(new PurchaseOrderPendingModel(purchaseOrder));
                    continue;
                }

                if (purchaseOrder.Status == PurchaseOrderStatus.CompliancePending && isCompliance)
                    response.Data.Add(new PurchaseOrderPendingModel(purchaseOrder));

                if (purchaseOrder.Status == PurchaseOrderStatus.ComercialPending && purchaseOrder.Area?.ResponsableUserId == user.Id)
                    response.Data.Add(new PurchaseOrderPendingModel(purchaseOrder));

                if (purchaseOrder.Status == PurchaseOrderStatus.OperativePending && purchaseOrder.PurchaseOrderAnalytics.Any(s => s.Analytic?.Sector?.ResponsableUserId == user.Id))
                    response.Data.Add(new PurchaseOrderPendingModel(purchaseOrder));

                if (purchaseOrder.Status == PurchaseOrderStatus.DafPending && isDaf)
                    response.Data.Add(new PurchaseOrderPendingModel(purchaseOrder));
            }

            return response;
        }

        private PurchaseOrderHistory GetHistory(PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model)
        {
            var history = new PurchaseOrderHistory
            {
                From = purchaseOrder.Status,
                PurchaseOrderId = purchaseOrder.Id,
                UserId = userData.GetCurrentUser().Id,
                CreatedDate = DateTime.UtcNow,
                Comment = model.Comments
            };

            return history;
        }

        public async Task<Response<File>> AttachFile(int purchaseOrderId, Response<File> response, IFormFile file, string userName)
        {
            var purchaseOrder = PurchaseOrderValidationHelper.Find(purchaseOrderId, response, unitOfWork);

            if (response.HasErrors()) return response;

            var fileToAdd = new File();
            var lastDotIndex = file.FileName.LastIndexOf('.');

            fileToAdd.FileName = file.FileName;
            fileToAdd.FileType = file.FileName.Substring(lastDotIndex);
            fileToAdd.InternalFileName = Guid.NewGuid();
            fileToAdd.CreationDate = DateTime.UtcNow;
            fileToAdd.CreatedUser = userName;

            purchaseOrder.File = fileToAdd;

            try
            {
                var fileName = $"{fileToAdd.InternalFileName.ToString()}{fileToAdd.FileType}";

                using (var fileStream = new FileStream(Path.Combine(fileConfig.PurchaseOrdersPath, fileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                unitOfWork.FileRepository.Insert(fileToAdd);
                unitOfWork.PurchaseOrderRepository.Update(purchaseOrder);
                unitOfWork.Save();

                response.Data = fileToAdd;
                response.AddSuccess(Resources.Billing.PurchaseOrder.FileAdded);
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.SaveFileError);
                logger.LogError(e);
            }

            return response;
        }

        public Response<PurchaseOrder> GetById(int id)
        {
            var response = new Response<PurchaseOrder>();

            response.Data = PurchaseOrderValidationHelper.FindWithAnalytic(id, response, unitOfWork);

            return response;
        }

        public Response DeleteFile(int id)
        {
            var response = new Response();

            var purchaseOrder = PurchaseOrderValidationHelper.Find(id, response, unitOfWork);

            if (purchaseOrder.File == null)
            {
                response.AddError(Resources.Common.FileNotFound);
                return response;
            }

            try
            {
                var file = purchaseOrder.File;
                purchaseOrder.File = null;
                purchaseOrder.FileId = null;

                unitOfWork.PurchaseOrderRepository.Update(purchaseOrder);
                unitOfWork.FileRepository.Delete(file);

                var fileName = $"{file.InternalFileName.ToString()}{file.FileType}";
                var path = Path.Combine(fileConfig.PurchaseOrdersPath, fileName);

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

        public IList<PurchaseOrder> GetByService(string serviceId)
        {
            return unitOfWork.PurchaseOrderRepository.GetByService(serviceId);
        }

        public IList<PurchaseOrder> GetByServiceLite(string serviceId)
        {
            return unitOfWork.PurchaseOrderRepository.GetByServiceLite(serviceId);
        }

        private void Validate(PurchaseOrderModel model, Response<PurchaseOrder> response)
        {
            PurchaseOrderValidationHelper.ValidateNumber(response, model, unitOfWork);
            PurchaseOrderValidationHelper.ValidateAnalytic(response, model);
            PurchaseOrderValidationHelper.ValidateClient(response, model);
            PurchaseOrderValidationHelper.ValidateArea(response, model);
            PurchaseOrderValidationHelper.ValidateCurrency(response, model);
            PurchaseOrderValidationHelper.ValidateDates(response, model);
            PurchaseOrderValidationHelper.ValidateAmmount(response, model.AmmountDetails);
        }
    }
}