using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.Workflow;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IRefundService
    {
        Response<string> Add(RefundModel model);

        Response<List<Option>> GetStates();

        Task<Response<File>> AttachFile(int refundId, Response<File> response, IFormFile file);

        Response<List<RefundListResultModel>> GetByParameters(RefundListParameterModel model);

        Response<RefundEditModel> Get(int id);

        Response DeleteFile(int id, int fileId);

        Response<IList<WorkflowHistoryModel>> GetHistories(int id);

        Response Update(RefundModel model);

        Response Delete(int id);

        Response<IList<Option>> GetAnalitycs();
    }
}
