using System;
using System.Net;

namespace DotQueue.HostLib
{
    internal class SubscribersNotificationAdapter : ISubscribersNotificationAdapter
    {
        public void Notify(Subscriber client, string message)
        {
            try
            {
                var request = WebRequest.Create($"http://{client.IpAddress}:{client.Port}/{message}");
                request.Method = WebRequestMethods.Http.Get;
                request.GetResponse();
            }
            catch (Exception)
            {
                //TODO logging or remove subscriber!
            }
        }
    }
}