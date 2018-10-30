using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing.PurchaseOrder
{
    public interface IPurchaseOrderFileService
    {
        Response DeleteFile(int id);

        Task<Response<File>> AttachFile(int purchaseOrderId, Response<File> response, IFormFile file, string userName);

        Response<byte[]> Export();
    }
}
