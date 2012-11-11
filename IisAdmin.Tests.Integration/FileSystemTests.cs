using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.Permissions;
using NUnit.Framework;

namespace IisAdmin.Tests.Unit
{
    [TestFixture]
    public class FileSystemTests
    {
        [Test]
        public void When_Creating_Home_Directory__Then_It_Should_Have_The_Appropriate_Rights()
        {
            var username = string.Format("testUser{0}", DateTime.Now.Millisecond);
            var administration = new AdministrationService();
            var context = new PrincipalContext(ContextType.Machine);
            var user = new UserPrincipal(context)
                           {
                               Name = username,
                               UserCannotChangePassword = false,
                               PasswordNeverExpires = true,
                           };
            user.SetPassword("!Password123");
            user.Save();

            GroupPrincipal grp = GroupPrincipal.FindByIdentity(context, "IIS_IUSRS");
            if (grp != null)
            {
                grp.Members.Add(user);
                grp.Save();
            }

            Assert.IsNotNull(grp);
            string dir = Path.Combine(ConfigurationManager.AppSettings["HomeDirectory"], username);
            administration.MkDir(username, dir);

            bool exists = Directory.Exists(dir);
            Assert.IsTrue(exists);

            Directory.Delete(dir);
            user.Delete();
        }
    }
}