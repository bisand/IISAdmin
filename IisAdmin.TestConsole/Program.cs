using System;
using IisAdmin.TestConsole.ServiceReference1;

namespace IisAdmin.TestConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Running tests against a remote service...");
            Console.WriteLine("");

            var proxy = new AdministrationServiceClient();
            if (proxy.ClientCredentials != null)
            {
                proxy.ClientCredentials.UserName.UserName = "admin";
                proxy.ClientCredentials.UserName.Password = "password";
            }

            var simpleRandom = DateTime.Now.Millisecond;
            var username = string.Format("testUser{0}", simpleRandom);
            const string password = "!Password123";
            var fqdn = string.Format("www.testuser{0}.com", simpleRandom);

            // Test to see if we could add a new user.
            var addUser = proxy.AddUser(username, password, fqdn);
            if (addUser)
                Console.WriteLine("Successfully created user: {0}", username);

            // Test to see if we could reset permissions on home directory.
            var resetPermissions = proxy.ResetPermissions(username);
            if (resetPermissions)
                Console.WriteLine("Successfully reset permissions for user: {0}", username);

            // Test to see if we could add a new host on IIS.
            var addHost = proxy.AddHost(username, fqdn);
            if (addHost)
                Console.WriteLine("Successfully added host: {0} for user: {1}", fqdn, username);

            // Test to see if we could delete the newly created host from IIS.
            var delHost = proxy.DelHost(username, fqdn);
            if (delHost)
                Console.WriteLine("Successfully deleted host: {0} for user: {1}", fqdn, username);

            // Test to see if we could delete the newly created user from Windows.
            var delUser = proxy.DelUser(username);
            if (delUser)
                Console.WriteLine("Successfully deleted user: {0}", username);

            Console.WriteLine("");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }
    }
}