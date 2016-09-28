using System;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace DotQueue.HostLib
{
    public class HostParameters
    {
        public long ApiPort { get; set; }
        public long DashboardPort { get; set; }
    }

    public class Host
    {
        private HttpSelfHostServer _httpSelfHostServer;
        private IDisposable _staticFileHost;
        private HostParameters _parameters;

        public Host(HostParameters parameters)
        {
            _parameters = parameters;
        }

        public void Start()
        {
            StartApiHost();
            StartFileHost();
        }

        private void StartFileHost()
        {
            var url = $"http://*:{_parameters.DashboardPort}/";
            var fileSystem = new PhysicalFileSystem("");
            var options = new FileServerOptions
            {
                EnableDirectoryBrowsing = true,
                FileSystem = fileSystem
            };
            options.StaticFileOptions.ContentTypeProvider = new CustomContentTypeProvider();
            options.StaticFileOptions.OnPrepareResponse = context =>
            {
                context.OwinContext.Response.Headers.Add("Cache-Control", new[] {"no-cache","no-store","must-revalidate"});
                context.OwinContext.Response.Headers.Add("Pragma", new[] {"no-cache"});
                context.OwinContext.Response.Headers.Add("Expire", new[] {"0"});
            };
            _staticFileHost = WebApp.Start(url, builder => builder.UseFileServer(options));
        }

        private void StartApiHost()
        {
            HttpSelfHostConfiguration _configuration = new HttpSelfHostConfiguration($"http://0.0.0.0:{_parameters.ApiPort}");
            _configuration.Routes.MapHttpRoute("ApiDefault", "api/{controller}/{method}/{id}",
                new { id = RouteParameter.Optional });
            _httpSelfHostServer = new HttpSelfHostServer(_configuration);
            _httpSelfHostServer.OpenAsync().Wait();
        }

        public void Stop()
        {
            _httpSelfHostServer.CloseAsync();
            _httpSelfHostServer.Dispose();
            _staticFileHost.Dispose();
        }
    }
}