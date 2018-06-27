using System;
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
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
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

        public PurchaseOrderService(IUnitOfWork unitOfWork, ILogMailer<PurchaseOrderService> logger, IOptions<FileConfig> fileOptions, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            fileConfig = fileOptions.Value;
            this.userData = userData;
        }

        public Response<PurchaseOrder> Add(PurchaseOrderModel model)
        {
            var response = new Response<PurchaseOrder>();

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            {
                var domain = model.CreateDomain(userData.GetCurrentUser().UserName);

                unitOfWork.PurchaseOrderRepository.Insert(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.SaveSuccess);
                
                response.Data = domain;
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
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

            PurchaseOrderValidationHelper.ValidateAnalytic(response, model);

            if (response.HasErrors()) return response;

            try
            {
                var domain = PurchaseOrderValidationHelper.FindWithAnalytic(model.Id, response, unitOfWork);

                model.UpdateDomain(domain, userData.GetCurrentUser().UserName);

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

        public Response<List<PurchaseOrderSearchResult>> Search(SearchPurchaseOrderParams parameters)
        {
            var result = unitOfWork.PurchaseOrderRepository.Search(parameters);

            var response = new Response<List<PurchaseOrderSearchResult>>
            {
                Data = result.Select(x => new PurchaseOrderSearchResult(x)).ToList()
            };

            if (!result.Any())
                response.AddWarning(Resources.Billing.PurchaseOrder.SearchEmpty);

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

        private static void Validate(PurchaseOrderModel model, Response<PurchaseOrder> response)
        {
            PurchaseOrderValidationHelper.ValidateNumber(response, model);
            PurchaseOrderValidationHelper.ValidateAnalytic(response, model);
            PurchaseOrderValidationHelper.ValidateClient(response, model);
            PurchaseOrderValidationHelper.ValidateArea(response, model);
            PurchaseOrderValidationHelper.ValidateCurrency(response, model);
            PurchaseOrderValidationHelper.ValidateDates(response, model);
            PurchaseOrderValidationHelper.ValidateAmmount(response, model.AmmountDetails);
        }
    }
}