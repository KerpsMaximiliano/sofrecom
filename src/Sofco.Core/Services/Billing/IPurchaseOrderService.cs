using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.Billing;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Model.DTO;
using Sofco.Model.Models.Billing;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using PurchaseOrder = Sofco.Model.Models.Billing.PurchaseOrder;

namespace Sofco.Core.Services.Billing
{
    public interface IPurchaseOrderService
    {
        Response<PurchaseOrder> Add(PurchaseOrderModel model);

        Task<Response<File>> AttachFile(int purchaseOrderId, Response<File> response, IFormFile file, string userName);

        Response<PurchaseOrder> GetById(int id);

        Response<List<PurchaseOrderSearchResult>> Search(SearchPurchaseOrderParams parameters);

        Response DeleteFile(int id);

        IList<PurchaseOrder> GetByService(string serviceId);

        IList<PurchaseOrder> GetByServiceLite(string serviceId);

        Response Update(PurchaseOrderModel model);

        Response UpdateSolfac(int id, int solfacId);

        Response MakeAdjustment(int id, IList<PurchaseOrderAmmountDetailModel> details);

        Response ChangeStatus(int id, PurchaseOrderStatusParams model);
        ICollection<PurchaseOrderHistory> GetHistories(int id);
    }
}
