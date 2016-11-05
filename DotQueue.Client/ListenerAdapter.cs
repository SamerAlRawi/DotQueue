using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DotQueue.Client
{
    internal class ListenerAdapter<T> : IListenerAdapter<T>, IDisposable
    {
        private int _localPort;
        private bool _listenerStarted;
        private bool _running = true;

        public void StartListener(int port)
        {
            if (_listenerStarted)
                return;

            _localPort = port;
            _listenerStarted = true;
            Task.Run(() => StartListener());
        }

        public event EventHandler<QueueNotification> NotificationReceived;

        private void StartListener()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add($"http://*:{_localPort}/");
            listener.Start();
            while (_running)
                try
                {
                    {
                        HttpListenerContext context = listener.GetContext();
                        HttpListenerRequest request = context.Request;
                        ProcessRequest(request.Url.LocalPath);
                        HttpListenerResponse response = context.Response;
                        byte[] buffer = { 79, 75 };//OK
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }

                }
                catch (Exception)
                {

                }
            listener.Stop();
            listener.Close();
        }

        private void ProcessRequest(string message)
        {
            Console.WriteLine($"Message received: {message}");
            if (message.Contains("subscribtion_added"))
            {
                Notify(QueueNotification.SubscriptionConfirmed);
            }
            if (message.Contains("new_message"))
            {
                Notify(QueueNotification.NewMessage);
            }
        }

        private void Notify(QueueNotification notification)
        {
            if (NotificationReceived != null)
            {
                NotificationReceived(this, notification);
            }
        }

        public void Dispose()
        {
            _running = false;
        }
    }
}