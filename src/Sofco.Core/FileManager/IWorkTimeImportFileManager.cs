using System.Collections.Generic;
using System.IO;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.FileManager
{
    public interface IWorkTimeImportFileManager
    {
        void Import(int analyticId, MemoryStream memoryStream, Response<IList<WorkTimeImportResult>> response);
    }
}
