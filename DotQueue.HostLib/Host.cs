﻿using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using DotQueue.Ioc;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;
using WebApiContrib.Formatting.Jsonp;

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
        }
        
        private void StartApiHost()
        {
            HttpSelfHostConfiguration _configuration = new HttpSelfHostConfiguration($"http://0.0.0.0:{_parameters.ApiPort}");
            _configuration.Routes.MapHttpRoute("ApiDefault", "api/{controller}/{method}/{id}",
                new { id = RouteParameter.Optional });

            _configuration.Services.Replace(typeof(IAssembliesResolver), new CustomAssemblyResolver());
            _configuration.DependencyResolver = ContainerBuilder.GetContainer();
            _configuration.Formatters.Insert(0, new JsonpMediaTypeFormatter(new JsonMediaTypeFormatter(), "callback"));
            
            _httpSelfHostServer = new HttpSelfHostServer(_configuration);
            _httpSelfHostServer.OpenAsync().Wait();
        }

        public void Stop()
        {
            _httpSelfHostServer.CloseAsync();
            _httpSelfHostServer.Dispose();
        }
    }
}