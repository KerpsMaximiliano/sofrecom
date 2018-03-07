namespace Sofco.Common.Security.Interfaces
{
    public interface ISessionManager
    {
        string GetUserName();

        string GetUserEmail();

        string GetUserEmail(string text);
    }
}