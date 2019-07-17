using System.IO;

namespace Sofco.Core.FileManager
{
    public interface ISalaryAdvancementFileManager
    {
        void Import(MemoryStream memoryStream);
    }
}
