using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.BuyOrder;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.Core.Services.RequestNote
{
    public interface IBuyOrderService
    {
        Task<Response<List<File>>> AttachFiles(Response<List<File>> response, List<IFormFile> files);
        IList<BuyOrderGridModel> GetAll(BuyOrderGridFilters filters);
        Response<BuyOrderModel> GetById(int id);
        Response<string> Add(BuyOrderModel model);
        void SaveChanges(BuyOrderModel model, int nextStatus);
        Response<IList<Option>> GetStates();
    }
}
