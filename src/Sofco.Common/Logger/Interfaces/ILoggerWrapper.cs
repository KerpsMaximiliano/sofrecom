using System;

namespace Sofco.Common.Logger.Interfaces
{
    public interface ILoggerWrapper<T>
    {
        void LogError(string message);

        void LogError(string error, Exception exception);
    }
}