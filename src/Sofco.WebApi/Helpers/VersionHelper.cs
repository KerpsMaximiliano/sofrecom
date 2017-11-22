namespace Sofco.WebApi.Helpers
{
    public class VersionHelper
    {
        public static string Version { get; }

        private const int Major = 0;

        private const int Minor = 5;

        private const int Revision = 6;

        private const string AppVersionFormat = "{0}.{1}.{2}";

        static VersionHelper()
        {
            Version = string.Format(AppVersionFormat, Major, Minor, Revision);
        }
    }
}