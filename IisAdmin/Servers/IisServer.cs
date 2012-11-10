using System.Linq;
using Microsoft.Web.Administration;

namespace IisAdmin.Servers
{
    public class IisServer
    {
        public bool AddWebSite(string username, string fqdn, string homeDirectory, string bindingInformation)
        {
            using (var serverManager = new ServerManager())
            {
                var pool = serverManager.ApplicationPools.Add(fqdn);
                pool.ManagedRuntimeVersion = "v4.0";
                pool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                pool.ProcessModel.UserName = username;
                var site = serverManager.Sites.Add(fqdn, "http", bindingInformation, homeDirectory);
                var app = site.Applications.FirstOrDefault();
                app.ApplicationPoolName = pool.Name;
                
                serverManager.CommitChanges();
            }
            return true;
        }
    }
}