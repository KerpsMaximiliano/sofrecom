using System;

namespace Sofco.Core.Logger
{
    public interface ILogMailer<T>
    {
        void LogError(Exception exception);
    }
}