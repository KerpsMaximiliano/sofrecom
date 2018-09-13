using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Common;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.DAL.Rrhh
{
    public interface ICloseDateRepository : IBaseRepository<CloseDate>
    {
        IList<CloseDate> Get(int startMonth, int startYear, int endMonth, int endYear);
        CloseDatesSettings GetBeforeCurrentAndNext();
    }
}
