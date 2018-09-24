using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Rrhh
{
    public interface IRrhhService
    {
        Response<byte[]> GenerateTigerTxt();
    }
}
