using System;
using System.Configuration;
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
            #region Deprecated Stuff

            //var binding = new WSHttpBinding();
            //binding.Security.Mode = SecurityMode.Message;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            //binding.Security.Transport.Realm = "IISAdmin Authentication";
            //binding.Security.Message = new NonDualMessageSecurityOverHttp {
            //    ClientCredentialType = MessageCredentialType.UserName,
            //    EstablishSecurityContext = false,
            //    NegotiateServiceCredential = false
            //    //EstablishSecurityContext = true,
            //    //NegotiateServiceCredential = true
            //};

            ////var binding = new BasicHttpBinding();
            ////binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            ////binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            ////binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            ////binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            ////binding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;
            ////binding.Security.Transport.Realm = "IISAdmin Authentication";

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

            Host = new ServiceHost(typeof (AdministrationService));
            Host.Open();

        }

        public void Stop()
        {
            Host.Close();
        }
    }
}