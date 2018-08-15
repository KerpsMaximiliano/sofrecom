using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Core.FileManager
{
    public interface IWorkTimeFileManager
    {
        void Import(int analyticId, IFormFile file, Response<IList<WorkTimeImportResult>> response);
    }
}
