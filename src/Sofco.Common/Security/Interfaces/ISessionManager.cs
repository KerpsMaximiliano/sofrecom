namespace Sofco.Common.Security.Interfaces
{
    public interface ISessionManager
    {
        string GetUserName();

        string GetUserMail();

        string GetUserEmail(string text);
    }
}