using System.Collections.Generic;
using Sofco.Core.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Core.Services.Billing
{
    public interface IAreaService
    {
        Response<List<AreaModel>> GetAll();
    }
}