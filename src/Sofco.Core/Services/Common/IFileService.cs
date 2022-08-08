using System;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Common
{
    public interface IFileService
    {
        Response<byte[]> ExportFile(int id, string path);

        Response<Tuple<byte[], string>> GetFile(int id, string path);

        Response<Tuple<byte[], string>> GetFile(int id);
        bool HasFile(int invoiceId);
        Response Delete(int id);

    }
}
