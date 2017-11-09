namespace Sofco.WebApi.Helpers
{
    public class VersionHelper
    {
        static VersionHelper()
        {
            Version = string.Format(AppVersionFormat, Major, Minor, Revision);
        }

        private const int Major = 0;
        private const int Minor = 4;
        private const int Revision = 1;
        private const string AppVersionFormat = "{0}.{1}.{2}";

        public static string Version { get; private set; }
    }
}