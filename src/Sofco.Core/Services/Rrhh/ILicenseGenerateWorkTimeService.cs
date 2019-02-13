using System.Collections.Generic;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseGenerateWorkTimeService
    {
        void GenerateWorkTimes(License license);
    }
}
