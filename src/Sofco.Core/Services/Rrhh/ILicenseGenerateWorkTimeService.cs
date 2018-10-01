using Sofco.Domain.Models.Rrhh;

namespace Sofco.Core.Services.Rrhh
{
    public interface ILicenseGenerateWorkTimeService
    {
        void GenerateWorkTimes(License license);
    }
}
