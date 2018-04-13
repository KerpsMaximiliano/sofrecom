using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Billing;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.DTO;
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
        private readonly CrmConfig crmConfig;

        public PurchaseOrderService(IUnitOfWork unitOfWork, ILogMailer<PurchaseOrderService> logger, IOptions<FileConfig> fileOptions, IOptions<CrmConfig> crmOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.fileConfig = fileOptions.Value;
            this.crmConfig = crmOptions.Value;
        }

        public PurchaseOrderOptions GetFormOptions()
        {
            var options = new PurchaseOrderOptions
            {
                Sellers = new List<Option>(),
                Managers = new List<Option>()
            };

            options.Sellers.AddRange(unitOfWork.UserRepository.GetSellers().Select(x => new Option { Id = x.Id, Text = x.Name }));
            options.Managers.AddRange(unitOfWork.UserRepository.GetManagers().Select(x => new Option { Id = x.Id, Text = x.Name }));

            return options;
        }

        public async Task<Response<PurchaseOrder>> Add(PurchaseOrder domain)
        {
            var response = new Response<PurchaseOrder>();

            Validate(domain, response);

            if (response.HasErrors()) return response;

            try
            {
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

            await UpdateProjectOnCrm(response, domain.ProjectId, domain.Number);

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

            response.Data = PurchaseOrderValidationHelper.Find(id, response, unitOfWork);

            return response;
        }

        public Response Update(PurchaseOrder domain)
        {
            var response = new Response<PurchaseOrder>();

            PurchaseOrderValidationHelper.Exist(response, domain, unitOfWork);

            if (response.HasErrors()) return response;

            Validate(domain, response);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.PurchaseOrderRepository.Update(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.Billing.PurchaseOrder.SaveSuccess);

                response.Data = domain;
            }
            catch (Exception e)
            {
                response.AddError(Resources.Common.ErrorSave);
                logger.LogError(e);
            }

            return response;
        }

        public ICollection<PurchaseOrder> Search(SearchPurchaseOrderParams parameters)
        {
            return unitOfWork.PurchaseOrderRepository.Search(parameters);
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

        public ICollection<PurchaseOrder> GetByService(string serviceId)
        {
            return unitOfWork.PurchaseOrderRepository.GetByService(serviceId);
        }

        private static void Validate(PurchaseOrder domain, Response<PurchaseOrder> response)
        {
            PurchaseOrderValidationHelper.ValidateTitle(response, domain);
            PurchaseOrderValidationHelper.ValidateNumber(response, domain);
            PurchaseOrderValidationHelper.ValidateAnalytic(response, domain);
            PurchaseOrderValidationHelper.ValidateClient(response, domain);
            PurchaseOrderValidationHelper.ValidateProject(response, domain);
            PurchaseOrderValidationHelper.ValidateComercialManager(response, domain);
            PurchaseOrderValidationHelper.ValidateManager(response, domain);
            PurchaseOrderValidationHelper.ValidateArea(response, domain);
            PurchaseOrderValidationHelper.ValidateYear(response, domain);
        }

        private async Task UpdateProjectOnCrm(Response response, string projectId, string purchaseOrder)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                var data = $"purchaseOrder={purchaseOrder}";

                var urlPath = $"/api/Project/{projectId}/purchaseOrder";

                try
                {
                    var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var httpResponse = await client.PutAsync(urlPath, stringContent);

                    httpResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    logger.LogError(urlPath + "; data: " + data, ex);
                    response.AddWarning(Resources.Common.CrmGeneralError);
                }
            }
        }
    }
}