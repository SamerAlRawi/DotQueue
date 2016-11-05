using System.Linq;
using System.Net.NetworkInformation;
using NUnit.Framework;

namespace DotQueue.Client.Tests
{
    [TestFixture]
    public class LocalPortResolverTests
    {
        LocalPortResolver _resolver = new LocalPortResolver();

        [Test]
        public void FindPort_Returns_Available_TCP_Port()
        {
            for (int i = 0; i < 100; i++)
            {
                var port = _resolver.FindFreePort();
                Assert.IsTrue(PortIsAvailable(port));
            }
        }

        private bool PortIsAvailable(int port)
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            var currentlyUsedPorts = tcpConnInfoArray.Select(i => i.LocalEndPoint.Port);
            return !currentlyUsedPorts.Contains(port);
        }
    }
}
