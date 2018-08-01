using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IAreaService
    {
        Response<List<AreaModel>> GetAll();
        Response<AreaAdminModel> GetById(int id);
        Response Add(AreaAdminModel model);
        Response Update(AreaAdminModel model);
        Response Active(int id, bool active);
    }
}