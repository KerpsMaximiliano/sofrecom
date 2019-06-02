using Sofco.Core.FileManager;

namespace Sofco.Core.Services.Rrhh
{
    public interface IPrepaidFactory
    {
        IPrepaidFileManager GetInstance(string code);
    }
}
