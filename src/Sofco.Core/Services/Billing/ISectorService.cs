using System.Collections.Generic;
using Sofco.Core.Models.Admin;
using Sofco.Core.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface ISectorService
    {
        Response<List<SectorModel>> GetAll();

        Response<SectorAdminModel> GetById(int id);

        Response Add(SectorAdminModel model);

        Response Update(SectorAdminModel model);

        Response Active(int id, bool active);
    }
}