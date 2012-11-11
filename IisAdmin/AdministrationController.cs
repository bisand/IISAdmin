using System;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using IisAdmin.Interfaces;

namespace IisAdmin.ServiceControl
{
    public class AdministrationController
    {
        public ServiceHost Host { get; set; }

        public void Start()
        {
            var binding = new WSHttpBinding();
            //binding.Security.Mode = SecurityMode.Message;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //binding.Security.Message = new NonDualMessageSecurityOverHttp
            //                               {
            //                                   ClientCredentialType = MessageCredentialType.UserName,
            //                                   EstablishSecurityContext = true,
            //                                   NegotiateServiceCredential = true
            //                               };

            var url = ConfigurationManager.AppSettings["EndpointUri"];
            var endpointUri = new Uri(url);
            Host = new ServiceHost(typeof (Administration), endpointUri);
            Host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, "localhost");

            Host.AddServiceEndpoint(typeof (IAdministration), binding, "");
            var smb = new ServiceMetadataBehavior {HttpGetEnabled = true};
            Host.Description.Behaviors.Add(smb);
            Host.Open();
        }

        public void Stop()
        {
            Host.Close();
        }
    }
}