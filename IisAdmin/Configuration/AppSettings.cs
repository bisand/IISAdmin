using System.Configuration;

namespace IisAdmin.Configuration
{
    public static class AppSettings
    {
        public static string HomeDirectory
        {
            get { return ConfigurationManager.AppSettings["HomeDirectory"] ?? "C:\\inetpub"; }
        }
    }
}