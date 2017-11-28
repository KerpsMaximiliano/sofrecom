using System;

namespace Sofco.Common.Logger.Interfaces
{
    public interface ILoggerWrapper<T>
    {
        void LogError(string message);

        void LogError(Exception exception);

        void LogError(string message, Exception exception);
    }
}