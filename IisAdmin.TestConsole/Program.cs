using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using IisAdmin.Interfaces;

namespace IisAdmin.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a URI to serve as the base address
            var url = new Uri("http://localhost:1337/IisAdmin/Administration");

            //Create ServiceHost
            var host = new ServiceHost(typeof(Administration), url);

            //Add a service endpoint
            host.AddServiceEndpoint(typeof(IAdministration), new WSHttpBinding(), "");

            //Enable metadata exchange
            var smb = new ServiceMetadataBehavior { HttpGetEnabled = true };
            host.Description.Behaviors.Add(smb);

            //Start the Service
            host.Open();

            Console.WriteLine("Service is host at " + DateTime.Now.ToString());
            Console.WriteLine("Hosts is running... Press <Enter> key to stop");
            Console.ReadLine();
        }
    }
}
