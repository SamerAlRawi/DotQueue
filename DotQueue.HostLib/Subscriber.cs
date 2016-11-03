using System;

namespace DotQueue.HostLib
{
    public class Subscriber
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public string Category { get; set; }
        public DateTime LastNotified { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Subscriber)
            {
                var subscriber = obj as Subscriber;
                return subscriber.IpAddress == IpAddress &&
                       subscriber.Category == Category &&
                       subscriber.Port == Port;
            }
            return false;
        }
    }
}