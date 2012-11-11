using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using IisAdmin.Interfaces;
using IisAdmin.TestConsole.ServiceReference1;

namespace IisAdmin.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy = new AdministrationServiceClient();
            proxy.ClientCredentials.UserName.UserName = "admin";
            proxy.ClientCredentials.UserName.Password = "password";
            var addUser = proxy.AddUser("test", "test", "www.test.com");
        }
    }
}
