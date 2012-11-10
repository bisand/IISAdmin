using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Security.AccessControl;
using IisAdmin.Configuration;
using IisAdmin.Interfaces;

namespace IisAdmin
{
    public class Administration : IAdministration
    {
        #region IAdministration Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwd"></param>
        /// <param name="fqdn"></param>
        /// <returns></returns>
        public bool AddUser(string username, string passwd, string fqdn)
        {
            try
            {
                // Retrieving context from local machine.
                var context = new PrincipalContext(ContextType.Machine);
                // Create user an set some appropriate values.
                var user = new UserPrincipal(context)
                               {
                                   Name = username,
                                   UserCannotChangePassword = false,
                                   PasswordNeverExpires = true,
                               };
                user.SetPassword(passwd);

                // Save the newly created user.
                user.Save();

                // Adding new user to the IIS_IUSRS group. Maybe we should add a separate group for each user. Maybe in some other release...
                GroupPrincipal grp = GroupPrincipal.FindByIdentity(context, "IIS_IUSRS");
                if (grp != null)
                {
                    grp.Members.Add(user);
                    grp.Save();
                }

                AddUserRemote(username, passwd);

                string homeDirectory = Path.Combine(AppSettings.HomeDirectory, username);
                MkDir(username, homeDirectory);

                return true;
            }
            catch (Exception ex)
            {
                // Here we could catch different exceptions, and maybe return a more sensable error response.
                Debug.WriteLine("An error occured while creating the new user {0}. Exception:{1}", username, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool DelUser(string username)
        {
            try
            {
                // Retrieving context from local machine.
                var context = new PrincipalContext(ContextType.Machine);
                // Locate the user.
                UserPrincipal user = UserPrincipal.FindByIdentity(context, username);
                if (user != null)
                {
                    // Delete the user if it exists.
                    user.Delete();
                }

                DelUserRemote(username);

                // It's debatable if we should return true if the user does not exist in the first place...
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occured while trying to delete the user {0}. Exception:{1}", username,
                                ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public bool SetPasswd(string username, string passwd)
        {
            try
            {
                // Retrieving context from local machine.
                var context = new PrincipalContext(ContextType.Machine);
                // Locate the user.
                UserPrincipal user = UserPrincipal.FindByIdentity(context, username);
                if (user != null)
                {
                    // Delete the user if it exists.
                    user.SetPassword(passwd);
                }

                SetPasswdRemote(username, passwd);

                // It's debatable if we should return true if the user does not exist in the first place...
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occured while trying to delete the user {0}. Exception:{1}", username,
                                ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public bool MkDir(string username, string directory)
        {
            DirectoryInfo info = Directory.CreateDirectory(directory);
            DirectorySecurity security = info.GetAccessControl();
            security.AddAccessRule(new FileSystemAccessRule(username,
                                                            FileSystemRights.Read |
                                                            FileSystemRights.Write |
                                                            FileSystemRights.Modify |
                                                            FileSystemRights.CreateDirectories |
                                                            FileSystemRights.CreateFiles |
                                                            FileSystemRights.ReadAndExecute
                                                            , AccessControlType.Allow));
            info.SetAccessControl(security);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool ResetPermissions(string username)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="fqdn"></param>
        /// <returns></returns>
        public bool AddHost(string username, string fqdn)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="fqdn"></param>
        /// <returns></returns>
        public bool DelHost(string username, string fqdn)
        {
            return true;
        }

        #endregion

        private void AddUserRemote(string username, string passwd)
        {
            // Here we could add the user to a remote machine where we could store its files and folders...
            // Since we do not know wether it is a windows or linux machine, we leave it for later to implement it.
        }

        private void DelUserRemote(string username)
        {
            // Here we could delete the user on a remote machine to disable access to its remote files and folders...
            // Since we do not know wether it is a windows or linux machine, we leave it for later to implement it.
        }

        private void SetPasswdRemote(string username, string passwd)
        {
            // Here we could set the password on the user on a remote machine...
            // Since we do not know wether it is a windows or linux machine, we leave it for later to implement it.
        }
    }
}