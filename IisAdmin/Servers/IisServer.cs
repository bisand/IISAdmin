using System.IO;
using System.Linq;
using Microsoft.Web.Administration;

namespace IisAdmin.Servers
{
    public class IisServer
    {
        public bool AddWebSite(string username, string password, string fqdn, string homeDirectory,
                               string bindingProtocol, string bindingInformation)
        {
            using (var serverManager = new ServerManager())
            {
                ApplicationPool pool = serverManager.ApplicationPools.Add(fqdn);
                pool.ManagedRuntimeVersion = "v4.0";
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.ApplicationPoolIdentity;
                }
                else
                {
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                    pool.ProcessModel.UserName = username;
                    pool.ProcessModel.Password = password;
                }
                Site site = serverManager.Sites.Add(fqdn, bindingProtocol, bindingInformation, homeDirectory);
                Application app = site.Applications.FirstOrDefault();
                app.ApplicationPoolName = pool.Name;

                serverManager.CommitChanges();
            }

            var filename = Path.Combine(homeDirectory, "index.html");
            using (StreamWriter sw = File.CreateText(filename))
            {
                sw.Write("<html><body><h1 style=\"font-family:'Segoe UI',Helvetica,Arial,sans-serif;\">Hello World!</h1></body></html>");
            }

            return true;
        }

        public bool DeleteWebSite(string username, string fqdn)
        {
            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites.FirstOrDefault(x => x.Name == fqdn);
                var pool = serverManager.ApplicationPools.FirstOrDefault(x => x.Name == fqdn);
                if (site != null)
                {
                    foreach (var app in site.Applications)
                    {
                        app.Delete();
                    }
                    site.Delete();
                }
                if (pool != null) 
                    pool.Delete();

                serverManager.CommitChanges();
            }
            return true;
        }
    }
}