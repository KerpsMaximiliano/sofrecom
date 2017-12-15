using Sofco.Model.Enums;

namespace Sofco.Core.StatusHandlers
{
    public interface ISolfacStatusFactory
    {
        ISolfacStatusHandler GetInstance(SolfacStatus status);
    }
}
