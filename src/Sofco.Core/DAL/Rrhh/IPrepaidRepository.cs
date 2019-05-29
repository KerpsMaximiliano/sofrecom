using System;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.DAL.Rrhh
{
    public interface IPrepaidImportedDataRepository : IBaseRepository<PrepaidImportedData>
    {
        IList<PrepaidDashboard> GetDashboard(int yearId, int monthId);
        IList<PrepaidImportedData> GetByDate(int yearId, int monthId);
        IList<PrepaidImportedData> GetByIds(IList<int> modelIds);
        void UpdateStatus(PrepaidImportedData prepaidImportedData);
        void DeleteByDateAndPrepaid(int prepaidId, DateTime dateTime);
        void Close(PrepaidImportedData prepaidImportedData);
        bool DateIsClosed(int prepaidId, int yearId, int monthId);
    }
}
