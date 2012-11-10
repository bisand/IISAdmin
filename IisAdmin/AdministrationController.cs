using System;
using System.Configuration;
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
            var url = new Uri(ConfigurationManager.AppSettings["EnpointUri"]);
            Host = new ServiceHost(typeof (Administration), url);
            Host.AddServiceEndpoint(typeof (IAdministration), new WSHttpBinding(), "");
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