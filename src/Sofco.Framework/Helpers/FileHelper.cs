using System;

namespace Sofco.Framework.Helpers
{
    public class FileHelper
    {
        public static string GenerateMailFileName(string directory)
        {
            var dateName = DateTime.Now.ToString("yyyyddMTHHmmss");

            return $@"{directory}\email.{dateName}.eml";
        }
    }
}
