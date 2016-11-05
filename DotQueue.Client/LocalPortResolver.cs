using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace DotQueue.Client
{
    internal class LocalPortResolver : ILocalPortResolver
    {
        private readonly Random _random;
        private readonly TcpConnectionInformation[] _tcpConnInfoArray;

        public LocalPortResolver()
        {
            _random = new Random(5000);
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            _tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
        }

        public int FindFreePort()
        {
            return FindAvailablePort();
        }

        private int FindAvailablePort()
        {
            int port = _random.Next(65536);
            var currentlyUsedPorts = _tcpConnInfoArray.Select(i => i.LocalEndPoint.Port);
            if (!currentlyUsedPorts.Contains(port))
            {
                return port;
            }
            return FindAvailablePort();
        }
    }
}