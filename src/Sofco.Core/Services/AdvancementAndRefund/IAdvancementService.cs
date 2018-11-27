using System.Collections.Generic;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.AdvancementAndRefund
{
    public interface IAdvancementService
    {
        Response<string> Add(AdvancementModel model);
        Response Update(AdvancementModel model);
        Response<AdvancementEditModel> Get(int id);
        Response<List<AdvancementListItem>> GetAllInProcess();
        Response<IList<AdvancementHistoryModel>> GetHistories(int id);
    }
}
