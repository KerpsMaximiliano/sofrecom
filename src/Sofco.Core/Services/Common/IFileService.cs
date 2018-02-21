using Sofco.Model.Utils;

namespace Sofco.Core.Services.Common
{
    public interface IFileService
    {
        Response<byte[]> ExportFile(int id);
    }
}
