using Microsoft.AspNetCore.Http;
using Sofco.Domain.Utils;

namespace Sofco.Core.FileManager
{
    public interface IPrepaidFileManager
    {
        Response Process(int yearId, int monthId, IFormFile file);
    }
}
