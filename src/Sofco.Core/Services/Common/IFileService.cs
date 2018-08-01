using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Common
{
    public interface IFileService
    {
        Response<byte[]> ExportFile(int id, string path);
    }
}
