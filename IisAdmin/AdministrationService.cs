using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using IisAdmin.Configuration;
using IisAdmin.Interfaces;
using IisAdmin.Servers;

namespace IisAdmin
{
    public class AdministrationService : IAdministrationService
    {
        #region IAdministrationService Members

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="passwd"> </param>
        /// <param name="fqdn"> </param>
        /// <returns> </returns>
        public bool AddUser(string username, string passwd, string fqdn)
        {
            try
            {
                var server = new IisServer();
                
                // Check if binding allready exists. If it does, return false.
                if(server.BindingExists(fqdn))
                    return false;

                // Retrieving context from local machine.
                var context = new PrincipalContext(ContextType.Machine);

                // Check if user allready exists. Return false if it does.
                var user = UserPrincipal.FindByIdentity(context, username);
                if (user != null)
                    return false;

                // Create user an set some appropriate values.
                user = new UserPrincipal(context)
                               {
                                   Name = username,
                                   UserCannotChangePassword = false,
                                   PasswordNeverExpires = true,
                               };
                user.SetPassword(passwd);

                // Save the newly created user.
                user.Save();

                // Adding new user to the IIS_IUSRS group. Maybe we should add a separate group for each user. Maybe in some other release...
                var grp = GroupPrincipal.FindByIdentity(context, "IIS_IUSRS");
                if (grp != null)
                {
                    grp.Members.Add(user);
                    grp.Save();
                }

                AddUserRemote(username, passwd);

                // Create the user's root directory.
                var rootHomeDirectory = Path.Combine(AppSettings.HomeDirectory, username);
                var mkDir = MkDir(username, rootHomeDirectory);

                // Adding a new application pool.
                var addApplicationPool = server.AddApplicationPool(username, passwd, fqdn);

                // Adding a new web site.
                var addSite = AddSite(username, fqdn);

                return mkDir & addApplicationPool & addSite;
            }
            catch (Exception ex)
            {
                // Here we could catch different exceptions, and maybe return a more sensable error response.
                Console.WriteLine("An error occured while creating the new user {0}. Exception:{1}", username, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <returns> </returns>
        public bool DelUser(string username)
        {
            try
            {
                // Retrieving context from local machine.
                var context = new PrincipalContext(ContextType.Machine);
                // Locate the user.
                var user = UserPrincipal.FindByIdentity(context, username);
                if (user != null)
                {
                    // Delete the user if it exists.
                    user.Delete();
                }

                DelUserRemote(username);

                var server = new IisServer();
                var deletedSites = server.DeleteUsersWebSites(username);

                // We leave the users files and folder intact for future storage.

                return deletedSites;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while trying to delete the user {0}. Exception:{1}", username,
                                ex.Message);
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="passwd"> </param>
        /// <returns> </returns>
        public bool SetPasswd(string username, string passwd)
        {
            try
            {
                // Retrieving context from local machine.
                var context = new PrincipalContext(ContextType.Machine);
                // Locate the user.
                var user = UserPrincipal.FindByIdentity(context, username);
                if (user != null)
                {
                    // Delete the user if it exists.
                    user.SetPassword(passwd);
                }

                SetPasswdRemote(username, passwd);

                var server = new IisServer();
                var pwdSet = server.SetPassword(username, passwd);

                return pwdSet;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while trying to delete the user {0}. Exception:{1}", username,
                                ex.Message);
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="directory"> </param>
        /// <returns> </returns>
        public bool MkDir(string username, string directory)
        {
            var info = Directory.CreateDirectory(directory);
            var security = info.GetAccessControl();
            security.AddAccessRule(new FileSystemAccessRule(username,
                                                            FileSystemRights.Read |
                                                            FileSystemRights.Write |
                                                            FileSystemRights.Modify |
                                                            FileSystemRights.CreateDirectories |
                                                            FileSystemRights.CreateFiles |
                                                            FileSystemRights.ReadAndExecute,
                                                            InheritanceFlags.ContainerInherit |
                                                            InheritanceFlags.ObjectInherit,
                                                            PropagationFlags.None,
                                                            AccessControlType.Allow));
            info.SetAccessControl(security);
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <returns> </returns>
        public bool ResetPermissions(string username)
        {
            try
            {
                var homeDirectory = Path.Combine(AppSettings.HomeDirectory, username);

                // Reset permissions on the directory and subdirectories. This could also be handled on remote locations...
                var info = new DirectoryInfo(homeDirectory);
                var security = info.GetAccessControl();
                security.AddAccessRule(new FileSystemAccessRule(username,
                                                                FileSystemRights.Read |
                                                                FileSystemRights.Write |
                                                                FileSystemRights.Modify |
                                                                FileSystemRights.CreateDirectories |
                                                                FileSystemRights.CreateFiles |
                                                                FileSystemRights.ReadAndExecute,
                                                                InheritanceFlags.ContainerInherit |
                                                                InheritanceFlags.ObjectInherit,
                                                                PropagationFlags.None,
                                                                AccessControlType.Allow));
                info.SetAccessControl(security);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="fqdn"> </param>
        /// <returns> </returns>
        public bool AddHost(string username, string fqdn)
        {
            try
            {
                var server = new IisServer();

                // Check if binding allready exists. If it does, return false.
                if (server.BindingExists(fqdn))
                    return false;

                // Adding a new binding/host.
                var addBinding = server.AddBinding(username, fqdn);

                return addBinding;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="fqdn"> </param>
        /// <returns> </returns>
        public bool DelHost(string username, string fqdn)
        {
            try
            {
                // Removing binding/host.
                var server = new IisServer();
                var removeBinding = server.RemoveBinding(username, fqdn);

                return removeBinding;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="fqdn"> </param>
        /// <returns> </returns>
        public bool AddSite(string username, string fqdn)
        {
            try
            {
                var server = new IisServer();

                // Check if binding allready exists. If it does, return false.
                if (server.BindingExists(fqdn))
                    return false;

                // Adding a new binding/host.
                var poolName = server.GetApplicationPoolName(username);

                // Create the site's root directory.
                var homeDirectory = Path.Combine(AppSettings.HomeDirectory, username, server.ReverseFqdn(fqdn));
                var mkDir = MkDir(username, homeDirectory);

                var addHost = server.AddHost(username, fqdn, poolName, homeDirectory, "http", string.Format("*:80:{0}", fqdn));

                return mkDir & addHost;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="username"> </param>
        /// <param name="fqdn"> </param>
        /// <returns> </returns>
        public bool DelSite(string username, string fqdn)
        {
            try
            {
                // Removing binding/host.
                var server = new IisServer();
                var deleteWebSite = server.DeleteWebSite(username, fqdn);

                return deleteWebSite;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
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