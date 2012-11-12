using System.ServiceModel;

namespace IisAdmin.Interfaces
{
    [ServiceContract]
    public interface IAdministrationService
    {
        [OperationContract]
        bool AddUser(string username, string passwd, string fqdn);

        [OperationContract]
        bool DelUser(string username);

        [OperationContract]
        bool SetPasswd(string username, string passwd);

        [OperationContract]
        bool ResetPermissions(string username);

        [OperationContract]
        bool AddHost(string username, string fqdn);

        [OperationContract]
        bool DelHost(string username, string fqdn);

        [OperationContract]
        bool AddSite(string username, string fqdn);

        [OperationContract]
        bool DelSite(string username, string fqdn);

        [OperationContract]
        bool MkDir(string username, string directory);
    }
}