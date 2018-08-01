using Sofco.Domain.Enums;

namespace Sofco.Core.StatusHandlers
{
    public interface ISolfacStatusFactory
    {
        ISolfacStatusHandler GetInstance(SolfacStatus status);
    }
}
