using System.Net;

namespace DotQueue.Client
{
    /// <summary>
    ///DotQueue service ddress configuration 
    /// </summary>
    public class DotQueueAddress
    {
        /// <summary>
        /// DotQueue service IP address
        /// </summary>
        public IPAddress IpAddress { get; set; }
        /// <summary>
        /// DotQueue listener Http port 
        /// </summary>
        public int Port { get; set; }
    }
}