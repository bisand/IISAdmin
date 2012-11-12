using System.IO;
using System.Linq;
using IisAdmin.Configuration;
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
                var pool = serverManager.ApplicationPools.Add(ReverseFqdn(fqdn));
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
                var site = serverManager.Sites.Add(ReverseFqdn(fqdn), bindingProtocol, bindingInformation, homeDirectory);
                var app = site.Applications.FirstOrDefault();
                app.ApplicationPoolName = pool.Name;

                serverManager.CommitChanges();
            }

            AddIndexFile(homeDirectory, fqdn);

            return true;
        }

        public bool AddApplicationPool(string username, string password, string fqdn)
        {
            using (var serverManager = new ServerManager())
            {
                var pool = serverManager.ApplicationPools.Add(ReverseFqdn(fqdn));
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

                serverManager.CommitChanges();
            }

            return true;
        }

        public bool AddHost(string username, string fqdn, string appPoolName, string homeDirectory, string bindingProtocol, string bindingInformation)
        {
            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites.Add(ReverseFqdn(fqdn), bindingProtocol, bindingInformation, homeDirectory);
                var app = site.Applications.FirstOrDefault();
                app.ApplicationPoolName = appPoolName;

                serverManager.CommitChanges();
            }

            AddIndexFile(homeDirectory, fqdn);

            return true;
        }

        public bool AddBinding(string username, string fqdn)
        {
            var result = false;
            using (var serverManager = new ServerManager())
            {
                var bindingInformation = string.Format("*:80:{0}", fqdn);
                const string bindingProtocol = "http";
                foreach (var site in serverManager.Sites)
                {
                    var app = site.Applications.FirstOrDefault();

                    if (app == null || string.IsNullOrWhiteSpace(app.ApplicationPoolName))
                        continue;

                    var pool = serverManager.ApplicationPools.FirstOrDefault(x => x.Name == app.ApplicationPoolName && x.ProcessModel.UserName == username);
                    if (pool == null)
                        continue;

                    var binding = site.Bindings.FirstOrDefault(x => x.BindingInformation == bindingInformation && x.Protocol == bindingProtocol);
                    if (binding == null)
                    {
                        site.Bindings.Add(bindingInformation, bindingProtocol);
                        result = true;
                    }
                }
                serverManager.CommitChanges();
            }
            return result;
        }

        public bool RemoveBinding(string username, string fqdn)
        {
            var result = false;
            using (var serverManager = new ServerManager())
            {
                var bindingInformation = string.Format("*:80:{0}", fqdn);
                const string bindingProtocol = "http";
                foreach (var site in serverManager.Sites)
                {
                    var app = site.Applications.FirstOrDefault();

                    if (app == null || string.IsNullOrWhiteSpace(app.ApplicationPoolName))
                        continue;

                    var pool = serverManager.ApplicationPools.FirstOrDefault(x => x.Name == app.ApplicationPoolName && x.ProcessModel.UserName == username);
                    if (pool == null)
                        continue;

                    var binding = site.Bindings.FirstOrDefault(x => x.BindingInformation == bindingInformation && x.Protocol == bindingProtocol);
                    if (binding != null)
                    {
                        binding.Delete();
                        result = true;
                    }
                }
                serverManager.CommitChanges();
            }
            return result;
        }

        public bool BindingExists(string fqdn)
        {
            var result = false;
            using (var serverManager = new ServerManager())
            {
                var bindingInformation = string.Format("*:80:{0}", fqdn);
                const string bindingProtocol = "http";
                foreach (var site in serverManager.Sites)
                {
                    var binding = site.Bindings.FirstOrDefault(x => x.BindingInformation == bindingInformation && x.Protocol == bindingProtocol);
                    if (binding != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SetPassword(string username, string password)
        {
            var result = false;
            using (var serverManager = new ServerManager())
            {
                foreach (var site in serverManager.Sites)
                {
                    var app = site.Applications.FirstOrDefault();

                    if (app == null || string.IsNullOrWhiteSpace(app.ApplicationPoolName))
                        continue;

                    var pool = serverManager.ApplicationPools.FirstOrDefault(x => x.Name == app.ApplicationPoolName && x.ProcessModel.UserName == username);
                    if (pool == null)
                        continue;

                    pool.ProcessModel.Password = password;
                    result = true;
                }
                serverManager.CommitChanges();
            }
            return result;
        }

        public bool DeleteWebSite(string username, string fqdn)
        {
            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites.FirstOrDefault(x => x.Name == ReverseFqdn(fqdn));
                if (site != null)
                {
                    foreach (var app in site.Applications)
                    {
                        app.Delete();
                    }
                    site.Delete();
                }

                serverManager.CommitChanges();
            }
            return true;
        }

        public bool DeleteUsersWebSites(string username)
        {
            using (var serverManager = new ServerManager())
            {
                foreach (var site in serverManager.Sites)
                {
                    var app = site.Applications.FirstOrDefault();

                    if (app == null || string.IsNullOrWhiteSpace(app.ApplicationPoolName))
                        continue;

                    var pool = serverManager.ApplicationPools.FirstOrDefault(x => x.Name == app.ApplicationPoolName && x.ProcessModel.UserName == username);
                    if (pool == null)
                        continue;

                    foreach (var a in site.Applications)
                    {
                        a.Delete();
                    }
                    site.Delete();
                }
                var pools = serverManager.ApplicationPools.Where(x => x.ProcessModel.UserName == username);
                foreach (var p in pools)
                {
                    p.Delete();
                }
                serverManager.CommitChanges();
            }
            return true;
        }

        public string GetHomeDirectory(string username)
        {
            using (var serverManager = new ServerManager())
            {
                foreach (var site in serverManager.Sites)
                {
                    var app = site.Applications.FirstOrDefault();

                    if (app == null || string.IsNullOrWhiteSpace(app.ApplicationPoolName))
                        continue;

                    var pool = serverManager.ApplicationPools.FirstOrDefault(x => x.Name == app.ApplicationPoolName && x.ProcessModel.UserName == username);
                    if (pool == null)
                        continue;

                    return app.Path;
                }
            }
            return string.Empty;
        }

        public string GetApplicationPoolName(string username)
        {
            using (var serverManager = new ServerManager())
            {
                var pool = serverManager.ApplicationPools.FirstOrDefault(x => x.ProcessModel.UserName == username);
                return pool != null ? pool.Name : string.Empty;
            }
        }

        public string WebSiteExists(string fqdn)
        {
            using (var serverManager = new ServerManager())
            {
                var pool = serverManager.Sites.FirstOrDefault(x => x.Name == ReverseFqdn(fqdn));
                return pool != null ? pool.Name : string.Empty;
            }
        }

        public string ReverseFqdn(string fqdn)
        {
            if (!AppSettings.ReverseFqdnInNames)
                return fqdn;

            var split = fqdn.Split('.');
            string reversed = split.Aggregate((workingSentence, next) => next + "." + workingSentence);
            return reversed;
        }

        private static void AddIndexFile(string homeDirectory, string fqdn)
        {
            var filename = Path.Combine(homeDirectory, "index.html");
            using (var sw = File.CreateText(filename))
            {
                sw.Write(string.Format("<html><body style=\"font-family:'Segoe UI',Helvetica,Arial,sans-serif;\"><h1>Hello World!</h1><p><em>From {0}</em></p></body></html>", fqdn));
            }
        }

    }
}