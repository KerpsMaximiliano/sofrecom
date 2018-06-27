﻿namespace Sofco.WebApi.Helpers
{
    public class VersionHelper
    {
        private const int Major = 1;

        private const int Minor = 37;

        private const int Revision = 166;

        private const string AppVersionFormat = "{0}.{1}.{2}";

        static VersionHelper()
        {
            Version = string.Format(AppVersionFormat, Major, Minor, Revision);
        }

        public static string Version { get; }
    }
}