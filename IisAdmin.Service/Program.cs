using IisAdmin.ServiceControl;
using Topshelf;

namespace IisAdmin.Service
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
                                {
                                    x.Service<AdministrationController>(s =>
                                                                {
                                                                    s.ConstructUsing(name => new AdministrationController());
                                                                    s.WhenStarted(tc => tc.Start());
                                                                    s.WhenStopped(tc => tc.Stop());
                                                                });
                                    x.RunAsLocalSystem();

                                    x.SetDescription("IIS Admin");
                                    x.SetDisplayName("IIS Admin");
                                    x.SetServiceName("iisadmin");
                                });
        }
    }
}
