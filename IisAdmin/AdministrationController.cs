using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using IisAdmin.Interfaces;

namespace IisAdmin.ServiceControl
{
    public class AdministrationController
    {
        public ServiceHost Host { get; set; }

        public void Start()
        {
            //ConfigureServiceHost();

            // Starting Service Host with a custom Username/Password binding instead of the standard WCF bindings....
            Host = new ServiceHost(typeof (AdministrationService));
            Host.Open();
            PrintUsage();
        }

        public void Stop()
        {
            Host.Close();
        }

        public void PrintUsage()
        {
            Console.WriteLine("");
            Console.WriteLine("Access web service from this url:");
            Console.WriteLine("{0}", Host.BaseAddresses.FirstOrDefault());
            Console.WriteLine("");
            Console.WriteLine("You can also install it as a service:");
            Console.WriteLine("");
            Console.WriteLine("IISAdmin.Service.exe install");
            Console.WriteLine("");
            Console.WriteLine("Type: IISAdmin.Service.exe --help for more info");
        }

        private void ConfigureServiceHost()
        {
            #region Deprecated Stuff. We moved to config file instead.

            //var binding = new WSHttpBinding();
            //binding.Security.Mode = SecurityMode.Message;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //binding.Security.Transport.Realm = "IISAdmin Authentication";
            //binding.Security.Message = new NonDualMessageSecurityOverHttp {
            //    ClientCredentialType = MessageCredentialType.UserName,
            //    EstablishSecurityContext = false,
            //    NegotiateServiceCredential = false
            //};

            //var url = ConfigurationManager.AppSettings["EndpointUri"];
            //var endpointUri = new Uri(url);
            //Host = new ServiceHost(typeof (AdministrationService), endpointUri);
            //Host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            //Host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUsernamePasswordValidator();
            //Host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, "localhost");

            //Host.AddServiceEndpoint(typeof (IAdministrationService), binding, "");
            //var smb = new ServiceMetadataBehavior {HttpGetEnabled = true};
            //Host.Description.Behaviors.Add(smb);
            //Host.Open();

            #endregion
        }
    }
}