using System.Configuration;

namespace IisAdmin.Configuration
{
    public static class AppSettings
    {
        public static string HomeDirectory
        {
            get { return ConfigurationManager.AppSettings["HomeDirectory"] ?? "C:\\inetpub"; }
        }

        public static bool ReverseFqdnInNames
        {
            get
            {
                bool value;
                var tmp = ConfigurationManager.AppSettings["ReverseFqdnInNames"] ?? "true";
                return bool.TryParse(tmp, out value) && value;
            }
        }
    }
}