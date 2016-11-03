using System;

namespace DotQueue.HostLib
{
    public class ClientAddress
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Category { get; set; }
        public DateTime LastNotified { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ClientAddress)
            {
                var address = obj as ClientAddress;
                return address.IpAddress == IpAddress &&
                       address.Category == Category &&
                       address.Port == Port;
            }
            return false;
        }
    }
}