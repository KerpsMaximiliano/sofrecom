using System;

namespace Sofco.Core.Logger
{
    public interface ILogMailer<T>
    {
        void LogError(string message, Exception exception);

        void LogError(Exception exception);
    }
}