using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using IisAdmin.Servers;
using Microsoft.Web.Administration;
using NUnit.Framework;

namespace IisAdmin.Tests.Unit
{
    [TestFixture]
    public class IisTests
    {
        [Test]
        public void When_Creating_New_Site__It_Should_Be_Present_In_IIS()
        {
            var username = string.Format("testUser{0}", DateTime.Now.Millisecond);
            const string password = "!Password123";

            var administration = new AdministrationService();
            var context = new PrincipalContext(ContextType.Machine);
            var user = new UserPrincipal(context)
                           {
                               Name = username,
                               UserCannotChangePassword = false,
                               PasswordNeverExpires = true,
                           };
            user.SetPassword(password);
            user.Save();

            var grp = GroupPrincipal.FindByIdentity(context, "IIS_IUSRS");
            if (grp != null)
            {
                grp.Members.Add(user);
                grp.Save();
            }

            Assert.IsNotNull(grp);
            var dir = Path.Combine(ConfigurationManager.AppSettings["HomeDirectory"], username);

            var info = Directory.CreateDirectory(dir);
            var security = info.GetAccessControl();
            security.AddAccessRule(new FileSystemAccessRule(username,
                                                            FileSystemRights.Read |
                                                            FileSystemRights.Write |
                                                            FileSystemRights.Modify |
                                                            FileSystemRights.CreateDirectories |
                                                            FileSystemRights.CreateFiles |
                                                            FileSystemRights.ReadAndExecute,
                                                            InheritanceFlags.ContainerInherit |
                                                            InheritanceFlags.ObjectInherit,
                                                            PropagationFlags.None,
                                                            AccessControlType.Allow));
            info.SetAccessControl(security);

            var server = new IisServer();

            // In order to make this work, you will have to add an entry to your host file or dns...
            const string fqdn = "www.test.com";
            server.AddWebSite(username, password, fqdn, dir, "http", string.Format("*:80:{0}", fqdn));

            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites.FirstOrDefault(x => x.Name == fqdn);
                Assert.IsNotNull(site);

                var app = site.Applications.FirstOrDefault();
                Assert.IsNotNull(app);

                var pool = serverManager.ApplicationPools.FirstOrDefault(x => x.Name == fqdn);
                Assert.IsNotNull(pool);

                // Cleaning up...
                app.Delete();
                site.Delete();
                pool.Delete();

                serverManager.CommitChanges();
            }

            // Cleaning up...
            Directory.Delete(dir, true);
            user.Delete();
        }
    }
}