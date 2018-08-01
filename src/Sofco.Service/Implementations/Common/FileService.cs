using System;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Common;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.Billing;

namespace Sofco.Service.Implementations.Common
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<PurchaseOrderService> logger;

        public FileService(IUnitOfWork unitOfWork, ILogMailer<PurchaseOrderService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response<byte[]> ExportFile(int id, string path)
        {
            var response = new Response<byte[]>();

            var file = unitOfWork.FileRepository.GetSingle(x => x.Id == id);

            if (file == null)
            {
                response.AddError(Resources.Common.FileNotFound);
                return response;
            }

            try
            {
                var bytes = System.IO.File.ReadAllBytes($"{path}\\{file.InternalFileName}{file.FileType}");
                response.Data = bytes;
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }
    }
}
