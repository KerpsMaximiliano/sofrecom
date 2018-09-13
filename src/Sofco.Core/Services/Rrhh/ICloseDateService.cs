using System.Collections.Generic;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface ICloseDateService
    {
        Response Add(IList<CloseDate> model);

        Response<CloseDateModel> Get(int startMonth, int startYear, int endMonth, int endYear);
    }
}
