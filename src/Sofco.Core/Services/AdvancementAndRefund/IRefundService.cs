using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IRefundService
    {
        Response<string> Add(RefundModel model);

        Task<Response<File>> AttachFile(int refundId, Response<File> response, IFormFile file);

        Response<RefundEditModel> Get(int id);

        Response DeleteFile(int id, int fileId);

        Response<IList<WorkflowHistoryModel>> GetHistories(int id);
    }
}
