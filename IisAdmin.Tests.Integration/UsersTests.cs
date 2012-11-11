using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace IisAdmin.Tests.Unit
{
    [TestFixture]
    public class UsersTests
    {
        [Test]
        public void When_Adding_User__Then_It_Will_Be_Created_Under_IIS_IUSRS_Group()
        {
            var username = string.Format("testUser{0}",DateTime.Now.Millisecond);
            var administration = new AdministrationService();
            administration.AddUser(username, "!Password123", "www.test.com");

            var context = new PrincipalContext(ContextType.Machine);
            var grp = GroupPrincipal.FindByIdentity(context, "IIS_IUSRS");

            Assert.IsNotNull(grp);

            var user = grp.Members.FirstOrDefault(x => x.Name == username);

            Assert.IsNotNull(user);
            user.Delete();
        }

        [Test]
        public void When_Deleting_User__Then_It_Should_Removed_And_Dissappear_From_IIS_IUSRS_Group()
        {
            var username = string.Format("testUser{0}", DateTime.Now.Millisecond);
            var administration = new AdministrationService();
            var userAdded = administration.AddUser(username, "!Password123", "");
            Assert.IsTrue(userAdded);

            var userDeleted = administration.DelUser(username);
            Assert.IsTrue(userDeleted);

            var context = new PrincipalContext(ContextType.Machine);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, username);
            Assert.IsNull(user);

            var grp = GroupPrincipal.FindByIdentity(context, "IIS_IUSRS");
            Assert.IsNotNull(grp);
            user = (UserPrincipal)grp.Members.FirstOrDefault(x => x.Name == username);
            Assert.IsNull(user);
        }

        [Test]
        public void When_Setting_NewPassword_On_User__Then_We_Should_Be_Able_To_Logon_With_New_Password()
        {
            var username = string.Format("testUser{0}", DateTime.Now.Millisecond);
            var administration = new AdministrationService();
            var userAdded = administration.AddUser(username, "!Password123", "");
            Assert.IsTrue(userAdded);

            var context = new PrincipalContext(ContextType.Machine);
            UserPrincipal user = UserPrincipal.FindByIdentity(context, username);
            Assert.IsNotNull(user);

            var validated = user.Context.ValidateCredentials(username, "!Password123");
            Assert.IsTrue(validated);

            var passwdChanged = administration.SetPasswd(username, "!Password321");
            Assert.IsTrue(passwdChanged);

            validated = user.Context.ValidateCredentials(username, "!Password321");
            Assert.IsTrue(validated);

            var userDeleted = administration.DelUser(username);
            Assert.IsTrue(userDeleted);
        }
    }
}
