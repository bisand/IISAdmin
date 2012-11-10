using IisAdmin.Servers;
using NUnit.Framework;

namespace IisAdmin.Tests.Unit
{
    [TestFixture]
    public class IisTests
    {
        [Test]
         public void When_Creating_New_Site__It_Should_Be_Present_In_IIS()
         {
             var administration = new Administration();
             var server = new IisServer();
             server.AddWebSite("");
         }
    }
}