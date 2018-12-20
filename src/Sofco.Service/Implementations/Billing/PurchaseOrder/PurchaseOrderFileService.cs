using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Services.Billing.PurchaseOrder;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.Billing;

namespace Sofco.Service.Implementations.Billing.PurchaseOrder
{
    public class PurchaseOrderFileService : IPurchaseOrderFileService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<PurchaseOrderFileService> logger;
        private readonly FileConfig fileConfig;
        private readonly IPurchaseOrderFileManager purchaseOrderFileManager;

        public PurchaseOrderFileService(IUnitOfWork unitOfWork,
            ILogMailer<PurchaseOrderFileService> logger,
            IOptions<FileConfig> fileOptions,
            IPurchaseOrderFileManager purchaseOrderFileManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            fileConfig = fileOptions.Value;
            this.purchaseOrderFileManager = purchaseOrderFileManager;
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

                if (File.Exists(path))
                {
                    File.Delete(path);
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

        public async Task<Response<Domain.Models.Common.File>> AttachFile(int purchaseOrderId, Response<Domain.Models.Common.File> response, IFormFile file, string userName)
        {
            var purchaseOrder = PurchaseOrderValidationHelper.Find(purchaseOrderId, response, unitOfWork);

            if (response.HasErrors()) return response;

            var fileToAdd = new Domain.Models.Common.File();
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

        public Response<byte[]> Export()
        {
            var response = new Response<byte[]>();

            try
            {
                var list = unitOfWork.PurchaseOrderRepository.GetForReport();

                var excel = purchaseOrderFileManager.CreateReport(list);

                response.Data = excel.GetAsByteArray();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ExportFileError);
            }

            return response;
        }
    }
}
