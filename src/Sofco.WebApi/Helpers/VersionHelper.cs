﻿namespace Sofco.WebApi.Helpers
{
    public class VersionHelper
    {
        private const int Major = 0;
        private const int Minor = 1;
        private const int Revision = 2;
        private const string AppVersionFormat = "{0}.{1}.{2}";

        public static string Version { get; private set; }

        static VersionHelper()
        {
            Version = string.Format(AppVersionFormat, Major, Minor, Revision);
        }
    }
}