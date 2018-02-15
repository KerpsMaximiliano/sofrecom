using System.Collections;
using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface ICostCenterService
    {
        Response Add(CostCenter domain);
        ICollection<CostCenter> GetAll();
        Response Active(int id, bool active);
        Response<CostCenter> GetById(int id);
        Response Edit(int id, string description);
    }
}
